using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AdaptaPE.Core;
using AdaptaPE.Windows;
using Forms=System.Windows.Forms;

namespace AdaptaPE
{
    public sealed class MainWindow:Window
    {
        internal static string ProductVersion {get{return ((System.Reflection.AssemblyInformationalVersionAttribute)Attribute.GetCustomAttribute(typeof(MainWindow).Assembly,typeof(System.Reflection.AssemblyInformationalVersionAttribute))).InformationalVersion.Split('+')[0];}}
        readonly Mascot mascot;ComboBox mascotChoice;CheckBox mascotLink;TextBlock mascotState;
        readonly App app;readonly Preferences settings=Preferences.Load();readonly Voice voice;readonly Filters filters=new Filters();readonly DesktopAccess desktop=new DesktopAccess();readonly Actions actions;
        Forms.NotifyIcon tray;Native.WinEventProc foregroundProc;IntPtr foregroundHook;HwndSource hwnd;FloatingWindow floating;HighlightWindow highlight;
        readonly DispatcherTimer choiceTimer=new DispatcherTimer{Interval=TimeSpan.FromSeconds(1)};
        bool suppress,suspended,quitting,cleaned,restoreVoice;int restoreFilter;string[] choices=new string[0];
        TextBlock status,result,transcript,voiceState;CheckBox voiceToggle;ComboBox filterChoice;ProgressBar microphoneLevel;TabControl tabs;WrapPanel choicePanel;
        public MainWindow(App app) {
            this.app=app;mascot=new Mascot(settings,Save);voice=new Voice(settings);actions=new Actions(desktop);
            Title="Adapta PE Lite · Windows";Width=800;Height=740;MinWidth=640;MinHeight=560;Icon=Theme.Asset("AdaptaPELite.ico");WindowStartupLocation=WindowStartupLocation.CenterScreen;
            FontFamily=new FontFamily("Segoe UI");FontSize=16;Background=new ImageBrush(Theme.Asset("background.png")){Stretch=Stretch.UniformToFill};Foreground=new SolidColorBrush(Color.FromRgb(27,39,44));
            Build();Closing+=(s,e)=>{if(!quitting){e.Cancel=true;Hide();if(settings.VoiceActive)ShowFloating();}};
            SourceInitialized+=(s,e)=>{hwnd=HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);if(hwnd==null)throw new InvalidOperationException("No se pudo obtener la ventana nativa de Adapta PE.");hwnd.AddHook(WindowMessage);if(!Native.RegisterHotKey(hwnd.Handle,160,0x4000|0x0002|0x0004,0x53))Report("Atajo de emergencia Ctrl+Shift+S ocupado; Detener todo sigue disponible en bandeja y panel.");};
        }
        void Do(Action action){if(!cleaned&&!Dispatcher.HasShutdownStarted){if(Dispatcher.CheckAccess())action();else Dispatcher.BeginInvoke(action);}}
        void Build() {
            var backdrop=new Grid{Background=new SolidColorBrush(Color.FromArgb(85,0,20,52))};Content=backdrop;
            var root=new Grid{Margin=new Thickness(22)};root.RowDefinitions.Add(new RowDefinition{Height=GridLength.Auto});root.RowDefinitions.Add(new RowDefinition());backdrop.Children.Add(root);
            var header=new DockPanel{Margin=new Thickness(0,0,0,18)};root.Children.Add(header);
            var panel=Button("Usar panel · PiP",ShowFloating);DockPanel.SetDock(panel,Dock.Right);header.Children.Add(panel);header.Children.Add(Theme.Logo(64));
            var title=new StackPanel();var name=Text("Adapta PE Lite",32,true);name.Foreground=Brushes.White;title.Children.Add(name);var subtitle=Text("Tu escritorio, a tu ritmo · Lite "+ProductVersion,16);subtitle.Foreground=Brushes.White;title.Children.Add(subtitle);header.Children.Add(title);
            var card=new Border{Background=new SolidColorBrush(Color.FromRgb(248,250,252)),CornerRadius=new CornerRadius(20),Padding=new Thickness(20)};Grid.SetRow(card,1);root.Children.Add(card);
            var inside=new Grid();inside.RowDefinitions.Add(new RowDefinition());inside.RowDefinitions.Add(new RowDefinition{Height=GridLength.Auto});card.Child=inside;
            tabs=new TabControl();inside.Children.Add(tabs);AddTab("Asistente de voz",VoicePage());AddTab("Filtros de color",FilterPage());AddTab("Mascota",MascotPage());AddTab("Ayuda",HelpPage());
            var footer=new StackPanel{Margin=new Thickness(0,14,0,0)};Grid.SetRow(footer,1);inside.Children.Add(footer);
            status=Text("Todo apagado. Activa lo que necesitas.",14);
#if !NET461
            AutomationProperties.SetLiveSetting(status,AutomationLiveSetting.Polite);
#endif
            footer.Children.Add(status);result=Text("",14);result.MaxHeight=70;footer.Children.Add(result);choicePanel=new WrapPanel();footer.Children.Add(choicePanel);
            var bottom=new DockPanel();var stop=Button("Detener todo",()=>StopAll(true),true);DockPanel.SetDock(stop,Dock.Right);bottom.Children.Add(stop);bottom.Children.Add(Text("Cerrar ajustes mantiene asistencia y panel.",14));footer.Children.Add(bottom);
        }
        static Expander More(StackPanel parent,string title){var item=new Expander{Header=title,Margin=new Thickness(0,12,0,8),FontWeight=FontWeights.SemiBold,MinHeight=44};parent.Children.Add(item);return item;}
        static TextBlock Text(string text,double size=15,bool bold=false){return new TextBlock{Text=text,FontSize=size,FontWeight=bold?FontWeights.SemiBold:FontWeights.Normal,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(0,0,0,10)};}
        static Button Button(string text,Action click,bool danger=false){var b=new Button{Content=text,Padding=new Thickness(14,8,14,8),Margin=new Thickness(0,0,8,8),MinHeight=44};if(danger){b.Background=new SolidColorBrush(Color.FromRgb(151,32,49));b.Foreground=Brushes.White;}AutomationProperties.SetName(b,text);b.Click+=(s,e)=>click();return b;}
        static StackPanel Page(){return new StackPanel{Margin=new Thickness(6,10,6,8)};}
        void AddTab(string title,StackPanel page){tabs.Items.Add(new TabItem{Header=title,Content=new ScrollViewer{Content=page,VerticalScrollBarVisibility=ScrollBarVisibility.Auto}});}
        void Heading(StackPanel page,string title,string description){page.Children.Add(Text(title,23,true));page.Children.Add(Text(description,14));}
        CheckBox Check(StackPanel page,string text,bool value,Action<bool> change){var c=new CheckBox{Content=text,IsChecked=value,Margin=new Thickness(0,4,0,14),Padding=new Thickness(4),MinHeight=44};c.Checked+=(s,e)=>{if(!suppress)change(true);};c.Unchecked+=(s,e)=>{if(!suppress)change(false);};page.Children.Add(c);return c;}
        TextBox Input(StackPanel page,string label,string value,Action<string> change){page.Children.Add(Text(label,14,true));var box=new TextBox{Text=value??"",MinHeight=36,Padding=new Thickness(8),Margin=new Thickness(0,0,0,14)};AutomationProperties.SetName(box,label);box.LostKeyboardFocus+=(s,e)=>{if(!suppress)Try(()=>{change(box.Text);Save();});};page.Children.Add(box);return box;}
        ComboBox Combo(StackPanel page,string label,string[] values,int selected,Action<int> change){page.Children.Add(Text(label,14,true));var box=new ComboBox{ItemsSource=values,SelectedIndex=selected,MinHeight=36,Margin=new Thickness(0,0,0,14)};AutomationProperties.SetName(box,label);box.SelectionChanged+=(s,e)=>{if(!suppress&&box.SelectedIndex>=0)Try(()=>{change(box.SelectedIndex);Save();});};page.Children.Add(box);return box;}
        StackPanel VoicePage(){var p=Page();Heading(p,"Control por voz","Di «Computadora, abre Bloc de notas». Reconocimiento y audio permanecen en tu equipo.");
            voiceToggle=Check(p,"Activar voz",false,v=>Try(()=>SetVoice(v)));voiceState=Text("Micrófono apagado",14);p.Children.Add(voiceState);
            p.Children.Add(Text("Señal del micrófono",14,true));microphoneLevel=new ProgressBar{Minimum=0,Maximum=1,Height=10,Margin=new Thickness(0,0,0,12)};AutomationProperties.SetName(microphoneLevel,"Nivel de audio del micrófono");p.Children.Add(microphoneLevel);
            transcript=Text("Aquí verás lo que escuché",14);transcript.MinHeight=44;p.Children.Add(transcript);
            var preference=Page();More(p,"Dispositivos y reconocimiento").Content=preference;
            var microphones=WaveInput.Devices();Combo(preference,"Micrófono",microphones,Math.Min(microphones.Length-1,Math.Max(0,settings.Microphone+1)),i=>{ChangeVoice(()=>settings.Microphone=i-1);});
            Combo(preference,"Reconocimiento de voz",new[]{"Automático · SAPI o local incluido","Local incluido · español","SAPI instalado en Windows"},Math.Max(0,Array.IndexOf(new[]{"auto","local","sapi"},settings.SpeechEngine)),i=>{ChangeVoice(()=>settings.SpeechEngine=new[]{"auto","local","sapi"}[i]);});
            Input(preference,"Nombre para llamar al asistente",settings.WakeWord,v=>{if(string.IsNullOrWhiteSpace(v)||v.Length>40)throw new ArgumentException("Usa un nombre de 1 a 40 caracteres.");ChangeVoice(()=>settings.WakeWord=v.Trim());});
            preference.Children.Add(Button("Permiso del micrófono",()=>OpenSettings("ms-settings:privacy-microphone")));preference.Children.Add(Button("Dispositivos de sonido",()=>OpenSettings("ms-settings:sound")));
            preference.Children.Add(Text("Local incluido funciona sin instalar un reconocedor de Windows. SAPI necesita un motor español; una voz TTS no basta.",14));
            var typed=Page();More(p,"Escribir una orden").Content=typed;typed.Children.Add(Text("Enfoca primero la aplicación de destino.",14));var command=new TextBox{MinHeight=44,Padding=new Thickness(8),Margin=new Thickness(0,0,0,12)};AutomationProperties.SetName(command,"Orden para la aplicación externa");typed.Children.Add(command);
            Action execute=()=>{string text=command.Text;command.Clear();Run(text,false);};command.KeyDown+=(s,e)=>{if(e.Key==System.Windows.Input.Key.Enter){e.Handled=true;execute();}};typed.Children.Add(Button("Ejecutar orden",execute));return p;
        }
        void OpenSettings(string uri){Try(()=>Process.Start(new ProcessStartInfo(uri){UseShellExecute=true}));}
        StackPanel FilterPage(){var p=Page();Heading(p,"Colores más cómodos","Siete matrices heredadas de Adapta PE. Son simulaciones de color, no una corrección médica.");
            filterChoice=Combo(p,"Filtro de pantalla",ColorMatrices.Titles,0,i=>{filters.Apply(i);settings.Filter=ColorMatrices.Names[i];Report(i==0?"Filtro retirado":ColorMatrices.Titles[i]+" activo · todos los monitores");});
            p.Children.Add(Button("Quitar filtro",()=>Try(()=>SetFilter(0))));
            p.Children.Add(Text("Los filtros se procesan localmente. En Windows 7 el filtro usa una capa de magnificación a 10 FPS; requiere comprobación en el equipo.",14));return p;
        }
        StackPanel MascotPage(){var p=Page();Heading(p,"Tu compañía peruana","Tinku, Cuchu o Yati. Puedes cambiarla u ocultarla cuando quieras.");
            mascotChoice=Combo(p,"Tu mascota",MascotCommand.Titles,Math.Max(0,Array.IndexOf(MascotCommand.Names,settings.Mascot)),i=>{mascot.Suspend(false);mascot.Choose(MascotCommand.Names[i]);});
            mascotState=Text("Sin mascota",14);p.Children.Add(mascotState);
            mascotLink=Check(p,"Mover mascota y panel juntos",settings.MascotLinked,v=>mascot.Link(v));
            p.Children.Add(Text("Unida: queda arriba del panel; arrastra cualquiera de sus títulos. Separada: conserva su lugar propio. Descansa tras 90 segundos sin uso y despierta al recibir una acción.",14));
            var row=new WrapPanel();foreach(var pair in new[]{Tuple.Create("Saludar","wave"),Tuple.Create("Dormir","sleep"),Tuple.Create("Despertar","wake")})row.Children.Add(Button(pair.Item1,()=>mascot.Gesture(pair.Item2)));row.Children.Add(Button("Mover con teclas",mascot.FocusHandle));row.Children.Add(Button("Otro monitor",mascot.NextScreen));p.Children.Add(row);
            p.Children.Add(Text("Arrastra su nombre entre monitores o usa las flechas con el nombre enfocado. Di «Computadora, mascota Cuchu» o «oculta mascota». Sólo se carga la elegida; el dibujo deja pasar tus clics.",14));return p;
        }
        StackPanel HelpPage(){var p=Page();Heading(p,"Empieza con una función","1. Enfoca una app. 2. Activa Voz. 3. Di «Computadora, muestra campos». 4. Elige un control. En Daltonismo puedes aplicar un filtro.");
            p.Children.Add(Text("Órdenes habituales\n• Abre Bloc de notas / busca movilidad en Wikipedia\n• Escribe Hola, José / escribe Hola en el campo Nombre\n• Selecciona Contacto / pulsa lo seleccionado\n• Lee la pantalla / silencio\n• Nueva pestaña / guardar / presiona control shift N\n• Filtro deuteranopía / quitar filtro\n• Detener todo",15));
            p.Children.Add(Text("Puedes decir «Computadora, por favor abre Bloc de notas» o «Computadora, quiero que abras Bloc de notas». Para dictar, di «escribe» y tu texto. Las palabras siguientes, incluso «por favor», forman parte del dictado.",15));
            p.Children.Add(Text("Emergencia: Ctrl+Shift+S detiene asistencia. También disponible en bandeja, panel y esta ventana. Windows puede impedir controlar aplicaciones elevadas; no es necesario desactivar UAC.",14));
            Check(p,"Iniciar Adapta PE Lite al entrar en Windows",settings.Login,v=>Try(()=>{Preferences.SetLogin(v,Process.GetCurrentProcess().MainModule.FileName);settings.Login=v;Save();}));
            Combo(p,"Esquina del panel",new[]{"Superior izquierda","Superior derecha","Inferior izquierda","Inferior derecha"},Math.Max(0,Array.IndexOf(new[]{"Superior izquierda","Superior derecha","Inferior izquierda","Inferior derecha"},settings.Corner)),i=>{settings.PanelFree=false;settings.Corner=new[]{"Superior izquierda","Superior derecha","Inferior izquierda","Inferior derecha"}[i];if(floating!=null)floating.Position();});
            var screens=Forms.Screen.AllScreens;Combo(p,"Monitor del panel",screens.Select((s,i)=>(i+1)+". "+s.DeviceName).ToArray(),Math.Max(0,Array.FindIndex(screens,s=>s.DeviceName==settings.Monitor)),i=>{settings.Monitor=screens[i].DeviceName;if(floating!=null)floating.Position();});
            p.Children.Add(Button("Salir de Adapta PE Lite",Exit));return p;
        }
        public void Initialize(){
            mascot.Status=text=>{mascotState.Text=text;bool old=suppress;suppress=true;mascotLink.IsChecked=settings.MascotLinked;mascotChoice.SelectedIndex=Math.Max(0,Array.IndexOf(MascotCommand.Names,settings.Mascot));suppress=old;};voice.SpeechOutput=active=>Do(()=>mascot.Speaking(voice.Speaking));voice.Wake=()=>mascot.React("phone");
            mascot.LayoutChanged=()=>{if(floating!=null){floating.Position();mascot.Attach(floating);}else mascot.Attach(null);};
            new WindowInteropHelper(this).EnsureHandle();Try(()=>filters.Recover());
            voice.Command=text=>Do(()=>{if(voice.Active)Run(text);});voice.Status=text=>Do(()=>{voiceState.Text=text;if(!voice.Active&&settings.VoiceActive){settings.VoiceActive=false;Save();}Report(text);Sync();});voice.Level=value=>Do(()=>microphoneLevel.Value=voice.Active?value:0);voice.Transcript=text=>Do(()=>{if(!voice.Active&&text.Length>0)return;transcript.Text=text;if(floating!=null)floating.Transcript.Text=text;});voice.FollowUp=()=>actions.PendingChoice;
            desktop.ReadResponse=text=>Do(()=>Speak(text));desktop.Highlight=rect=>Do(()=>{if(!rect.HasValue){if(highlight!=null)highlight.Hide();return;}if(highlight==null)highlight=new HighlightWindow();highlight.ShowRect(rect.Value);});
            actions.Respond=text=>Do(()=>Speak(text));actions.ChoiceLabels=values=>Do(()=>ShowChoices(values));actions.Local=LocalCommand;
            foregroundProc=(h,evt,w,o,c,t,time)=>Do(()=>{actions.OnForeground(w);if(w!=IntPtr.Zero&&!Native.Own(w)&&voice.Active&&IsVisible)ShowFloating();});foregroundHook=Native.SetWinEventHook(3,3,IntPtr.Zero,foregroundProc,0,0,0x2);actions.OnForeground(Native.GetForegroundWindow());
            tray=new Forms.NotifyIcon{Icon=new System.Drawing.Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"AdaptaPELite.ico")),Text="Adapta PE Lite",Visible=true};var menu=new Forms.ContextMenuStrip();menu.Items.Add("Abrir ajustes",null,(s,e)=>Do(ShowSettings));menu.Items.Add("Voz: activar / apagar",null,(s,e)=>Do(()=>Try(()=>SetVoice(!voice.Active))));var colors=new Forms.ToolStripMenuItem("Daltonismo");for(int i=0;i<ColorMatrices.Titles.Length;i++){int index=i;colors.DropDownItems.Add(ColorMatrices.Titles[i],null,(s,e)=>Do(()=>Try(()=>SetFilter(index))));}menu.Items.Add(colors);menu.Items.Add("Panel flotante",null,(s,e)=>Do(ShowFloating));menu.Items.Add("Detener todo",null,(s,e)=>Do(()=>StopAll(true)));menu.Items.Add("Salir",null,(s,e)=>Do(Exit));tray.ContextMenuStrip=menu;tray.DoubleClick+=(s,e)=>Do(ShowSettings);
            choiceTimer.Tick+=(s,e)=>{if(choices.Length>0&&!actions.PendingChoice)actions.CancelChoices();};if(!string.IsNullOrEmpty(Preferences.Notice))Report(Preferences.Notice);
        }
        void Run(string text,bool heard=true){if(string.IsNullOrWhiteSpace(text))return;var parsed=Commands.Parse(text);mascot.React("idle");mascot.PrepareSpeech(parsed.Kind=="readScreen"||parsed.Kind=="readElement");transcript.Text=(heard?"Escuché: ":"Orden: ")+text;result.Text=(Commands.Parse(text).Kind=="unknown"?"Buscando acción: ":"Orden detectada: ")+text;if(floating!=null){floating.Transcript.Text=transcript.Text;floating.Response.Text=result.Text;}_ = actions.Execute(text);}
        Task<bool> LocalCommand(Command command){return Task.FromResult(HandleLocal(command));}
        bool HandleLocal(Command command){
            switch(command.Kind){
                case "mascot":if(MascotCommand.Names.Contains(command.Text)){mascot.Suspend(false);mascot.Choose(command.Text);suppress=true;mascotChoice.SelectedIndex=Array.IndexOf(MascotCommand.Names,settings.Mascot);suppress=false;Save();}else mascot.Gesture(command.Text);Report(mascotState.Text);return true;
                case "stop":StopAll(true);return true;case "cancel":actions.Cancel();voice.Silence();Report("Acción cancelada");return true;
                case "voice":SetVoice(command.Text=="on");return true;
                case "silence":voice.Silence();Report("Lectura detenida");return true;
                case "filter":int index=Array.IndexOf(ColorMatrices.Names,Commands.Normalize(command.Text));if(index<0)throw new ArgumentException("Filtro desconocido.");SetFilter(index);return true;
                case "corner":string corner=Corners.FirstOrDefault(c=>Commands.Normalize(c)==command.Text);if(corner==null)throw new ArgumentException("Esquina no disponible.");settings.Corner=corner;settings.PanelFree=false;ShowFloating();Save();return true;
                case "app":if(command.Text=="mostrar ajustes"||command.Text=="abrir adapta")ShowSettings();else if(command.Text=="modo flotante")ShowFloating();else if(command.Text=="salir de adapta")Exit();else{Hide();if(floating!=null)floating.Hide();}return true;
                case "unsupported":Speak(command.Text);return true;
                default:return false;
            }
        }
        internal static readonly string[] PanelCorners={"Superior izquierda","Superior derecha","Inferior izquierda","Inferior derecha"};
        static readonly string[] Corners={"Superior izquierda","Superior derecha","Inferior izquierda","Inferior derecha"};
        void ChangeVoice(Action change){change();try{voice.Restart();}finally{settings.VoiceActive=voice.Active;Sync();Save();}if(!voice.Active)Report("Preferencia de voz guardada. Activa Voz para usarla.");}
        void SetVoice(bool value){if(suspended&&value)throw new InvalidOperationException("Sesión suspendida.");try{if(value)voice.Start();else voice.Stop();}catch(Exception error){voiceState.Text=error.Message;throw;}settings.VoiceActive=voice.Active;mascot.React(value?"phone":"idle");Sync();Save();if(!value){voiceState.Text="Micrófono apagado";Report("Voz apagada");}}
        void SetFilter(int index){filters.Apply(index);mascot.React("laptop");settings.Filter=ColorMatrices.Names[index];Sync();Save();Report(index==0?"Filtro retirado":ColorMatrices.Titles[index]+" activo · todos los monitores");}
        void Sync(){suppress=true;voiceToggle.IsChecked=voice.Active;filterChoice.SelectedIndex=filters.Selected;suppress=false;if(floating!=null)floating.Sync(voice.Active);}
        void Save(){Try(()=>settings.Save());}
        void Try(Action action){try{action();}catch(Exception e){ReportError(e);}}
        public void ReportError(Exception error){Report(error.Message);Sync();}
        void Report(string text){status.Text=text;if(floating!=null)floating.State.Text=text;}
        void Speak(string text){result.Text=text;if(floating!=null)floating.Response.Text=text;try{voice.Say(text);}catch(Exception e){Report("Respuesta escrita disponible; salida de voz no disponible: "+e.Message);}}
        void ShowChoices(string[] values){choices=values;if(values.Length>0)choiceTimer.Start();else choiceTimer.Stop();choicePanel.Children.Clear();for(int i=0;i<values.Length;i++){int index=i;choicePanel.Children.Add(Button(values[i],()=>Run("elige "+(index+1),false)));}if(floating!=null)floating.SetChoices(values,i=>Run("elige "+(i+1),false));}
        public void DisplayChanged(){mascot.Position();filters.DisplayChanged();if(floating!=null)floating.Position();if(highlight!=null)highlight.Hide();}
        public void ShowSettings(){Show();WindowState=WindowState.Normal;Activate();}
        void ShowFloating(){if(floating==null){floating=new FloatingWindow(settings,ShowSettings,()=>StopAll(true),v=>Try(()=>SetVoice(v)));floating.RunLocal=text=>Run(text,false);floating.Save=Save;floating.Interaction=mascot.Touch;floating.LocationChanged+=(s,e)=>mascot.Attach(floating);floating.IsVisibleChanged+=(s,e)=>mascot.Attach(floating);}floating.SetChoices(choices,i=>Run("elige "+(i+1),false));floating.Transcript.Text=transcript.Text;floating.Response.Text=result.Text;floating.State.Text=status.Text;floating.Show();floating.Position();mascot.Attach(floating);settings.Floating=true;Save();Sync();}
        public void StopAll(bool remember){
            mascot.React("idle");if(!remember)mascot.Suspend(true);
            actions.Cancel();voice.Stop();voice.Silence();voiceState.Text="Micrófono apagado";result.Text="";if(floating!=null)floating.Response.Text="";
            bool filterStopped=true;try{filters.Stop();}catch(Exception e){filterStopped=false;ReportError(e);}
            if(remember){restoreVoice=false;restoreFilter=0;settings.VoiceActive=false;settings.Filter=ColorMatrices.Names[filters.Selected];Save();}
            Sync();Report(filterStopped?"Asistencia detenida":"Voz detenida; el filtro no se pudo restaurar. Reintenta Detener todo.");
        }
        public void Restore(){mascot.Choose(settings.Mascot);bool wantsVoice=settings.VoiceActive;string wantsFilter=settings.Filter;if(settings.Floating)ShowFloating();if(wantsVoice)Try(()=>SetVoice(true));int index=Array.IndexOf(ColorMatrices.Names,wantsFilter);if(index>0)Try(()=>SetFilter(index));}
        public void Suspend(){if(suspended)return;restoreVoice=voice.Active;restoreFilter=filters.Selected;suspended=true;StopAll(false);Report("Sesión suspendida · micrófono y entrada detenidos");}
        public async void Resume(){if(!suspended)return;await Task.Delay(750);if(cleaned||!suspended||!Native.DefaultDesktop())return;suspended=false;mascot.Suspend(false);if(restoreVoice)Try(()=>SetVoice(true));if(restoreFilter>0)Try(()=>SetFilter(restoreFilter));Sync();}
        IntPtr WindowMessage(IntPtr h,int message,IntPtr w,IntPtr l,ref bool handled){if(message==0x312&&w.ToInt32()==160){StopAll(true);handled=true;}return IntPtr.Zero;}
        void Exit(){quitting=true;Save();StopAll(false);app.Shutdown();}
        public void Cleanup(){if(cleaned)return;StopAll(false);cleaned=true;choiceTimer.Stop();if(hwnd!=null)Native.UnregisterHotKey(hwnd.Handle,160);if(foregroundHook!=IntPtr.Zero)Native.UnhookWinEvent(foregroundHook);if(tray!=null){tray.Visible=false;tray.Dispose();}mascot.Dispose();voice.Dispose();Try(()=>filters.Dispose());actions.Dispose();if(floating!=null){floating.AllowClose=true;floating.Close();}if(highlight!=null)highlight.Close();}
    }
    sealed class FloatingWindow:Window {
        public Action<string> RunLocal;public Action Save,Interaction;public bool AllowClose;readonly Preferences settings;readonly WrapPanel choices=new WrapPanel();public readonly TextBlock State=new TextBlock(),Transcript=new TextBlock(),Response=new TextBlock();
        readonly CheckBox voiceToggle=new CheckBox{Content="Voz"};bool syncing;bool positioning;
        public FloatingWindow(Preferences settings,Action show,Action stop,Action<bool> voice){this.settings=settings;Title="Adapta PE Lite · panel";Icon=Theme.Asset("AdaptaPELite.ico");Width=350;SizeToContent=SizeToContent.Height;ShowInTaskbar=false;Topmost=true;ShowActivated=false;ResizeMode=ResizeMode.NoResize;WindowStyle=WindowStyle.ToolWindow;MaxHeight=660;FontFamily=new FontFamily("Segoe UI");FontSize=15;Background=new SolidColorBrush(Color.FromRgb(248,250,252));Foreground=new SolidColorBrush(Color.FromRgb(27,39,44));
            var shell=new DockPanel{LastChildFill=true};Content=shell;var page=new StackPanel{Margin=new Thickness(18)};var scroll=new ScrollViewer{Content=page,VerticalScrollBarVisibility=ScrollBarVisibility.Auto};
            var brand=new StackPanel{Orientation=Orientation.Horizontal,Margin=new Thickness(0,0,0,14)};brand.Children.Add(Theme.Logo(44));var heading=new StackPanel();heading.Children.Add(new TextBlock{Text="Adapta PE Lite",FontSize=22,FontWeight=FontWeights.SemiBold});heading.Children.Add(new TextBlock{Text="A tu ritmo · Lite "+MainWindow.ProductVersion,FontSize=12,Foreground=Theme.Blue});brand.Children.Add(heading);page.Children.Add(brand);
            State.Text="Activa sólo lo que necesitas";State.FontSize=13;State.Foreground=new SolidColorBrush(Color.FromRgb(71,85,105));State.TextWrapping=TextWrapping.Wrap;State.Margin=new Thickness(0,0,0,10);page.Children.Add(State);
            foreach(var toggle in new[]{voiceToggle}){toggle.MinHeight=44;AutomationProperties.SetName(toggle,toggle.Content.ToString());page.Children.Add(toggle);}
            voiceToggle.Checked+=(s,e)=>{if(!syncing)voice(true);};voiceToggle.Unchecked+=(s,e)=>{if(!syncing)voice(false);};
            var options=new Menu{Margin=new Thickness(0,0,0,8)};var menu=new MenuItem{Header="Opciones del panel"};options.Items.Add(menu);page.Children.Add(options);
            foreach(var corner in MainWindow.PanelCorners){string value=corner;var item=new MenuItem{Header=value};item.Click+=(s,e)=>{settings.PanelFree=false;settings.Corner=value;Position();Save?.Invoke();};menu.Items.Add(item);}
            var monitorItem=new MenuItem{Header="Otro monitor"};monitorItem.Click+=(s,e)=>NextScreen();menu.Items.Add(monitorItem);
            var petItem=new MenuItem{Header="Mascota"};menu.Items.Add(petItem);foreach(string name in MascotCommand.Names){string value=name;var item=new MenuItem{Header=MascotCommand.Titles[Array.IndexOf(MascotCommand.Names,value)]};item.Click+=(s,e)=>RunLocal(value=="ninguna"?"oculta mascota":"mascota "+value);petItem.Items.Add(item);}
            var linked=new MenuItem{Header="Mover mascota y panel juntos",IsCheckable=true};linked.Click+=(s,e)=>RunLocal(linked.IsChecked?"une mascota al panel":"separa mascota del panel");menu.Items.Add(linked);
            menu.SubmenuOpened+=(s,e)=>{linked.IsChecked=settings.MascotLinked;linked.IsEnabled=settings.Mascot!="ninguna";};
            var colorItem=new MenuItem{Header="Filtro de pantalla"};menu.Items.Add(colorItem);for(int i=0;i<ColorMatrices.Names.Length;i++){int index=i;var item=new MenuItem{Header=ColorMatrices.Titles[index]};item.Click+=(s,e)=>RunLocal("filtro "+ColorMatrices.Names[index]);colorItem.Items.Add(item);}
            var heard=new Border{Background=Brushes.White,CornerRadius=new CornerRadius(12),Padding=new Thickness(12),Margin=new Thickness(0,10,0,8)};var responses=new StackPanel();heard.Child=responses;page.Children.Add(heard);
            foreach(var text in new[]{Transcript,Response}){text.TextWrapping=TextWrapping.Wrap;text.Margin=new Thickness(0,0,0,6);text.MaxHeight=78;responses.Children.Add(text);}Transcript.FontSize=13;Transcript.Foreground=new SolidColorBrush(Color.FromRgb(71,85,105));Response.FontWeight=FontWeights.SemiBold;
            page.Children.Add(choices);
            var buttons=new Grid();buttons.ColumnDefinitions.Add(new ColumnDefinition());buttons.ColumnDefinitions.Add(new ColumnDefinition());var settingsButton=new Button{Content="Ajustes",Margin=new Thickness(0,0,6,0)};settingsButton.Click+=(s,e)=>show();buttons.Children.Add(settingsButton);var stopButton=new Button{Content="Detener todo",Margin=new Thickness(6,0,0,0),Background=new SolidColorBrush(Color.FromRgb(151,32,49)),Foreground=Brushes.White};stopButton.Click+=(s,e)=>stop();Grid.SetColumn(stopButton,1);buttons.Children.Add(stopButton);buttons.Margin=new Thickness(18,8,18,18);DockPanel.SetDock(buttons,Dock.Bottom);shell.Children.Add(buttons);shell.Children.Add(scroll);
            SourceInitialized+=(s,e)=>{HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WindowMoved);var h=new WindowInteropHelper(this).Handle;var style=Native.GetWindowLongPtr(h,-20);Native.SetWindowLongPtr(h,-20,new IntPtr(style.ToInt64()|0x08000000));};SizeChanged+=(s,e)=>Position();
            Closing+=(s,e)=>{if(!AllowClose&&!Application.Current.Dispatcher.HasShutdownStarted&&Application.Current.Windows.OfType<MainWindow>().Any(w=>w.IsLoaded)){e.Cancel=true;Hide();settings.Floating=false;Save?.Invoke();}};
        }
        public void Sync(bool voice){syncing=true;voiceToggle.IsChecked=voice;syncing=false;}
        public void SetChoices(string[] values,Action<int> choose){choices.Children.Clear();for(int i=0;i<values.Length;i++){int index=i;var b=new Button{Content=values[i],Padding=new Thickness(8),Margin=new Thickness(0,0,4,6),MaxWidth=270};b.Click+=(s,e)=>choose(index);choices.Children.Add(b);}}
        double Scale {get{var source=PresentationSource.FromVisual(this);return source!=null&&source.CompositionTarget!=null?source.CompositionTarget.TransformToDevice.M11:1;}}
        int Reserved {get{return settings.MascotLinked&&settings.Mascot!="ninguna"?(int)Math.Round(164*Scale):0;}}
        IntPtr WindowMoved(IntPtr h,int message,IntPtr w,IntPtr l,ref bool handled){if(message==0x232){Interaction?.Invoke();SavePosition();}return IntPtr.Zero;}
        public void NextScreen(){var screens=Forms.Screen.AllScreens;if(screens.Length<2){State.Text="Sólo hay una pantalla conectada";return;}int index=Array.FindIndex(screens,s=>s.DeviceName==settings.Monitor);settings.Monitor=screens[(index+1)%screens.Length].DeviceName;Position();Save?.Invoke();}
        public void MoveBy(int dx,int dy){var h=new WindowInteropHelper(this).Handle;Native.Rect rect;Native.GetWindowRect(h,out rect);var screen=Mouse.LeftButton==MouseButtonState.Pressed?Forms.Screen.FromPoint(Forms.Cursor.Position):Forms.Screen.FromHandle(h);var b=screen.WorkingArea;int x=Math.Max(b.Left,Math.Min(b.Right-(rect.Right-rect.Left),rect.Left+dx)),y=Math.Max(b.Top+Reserved,Math.Min(b.Bottom-(rect.Bottom-rect.Top),rect.Top+dy));Native.SetWindowPos(h,new IntPtr(-1),x,y,0,0,0x11);}
        public void SavePosition(){if(positioning||!IsVisible)return;var h=new WindowInteropHelper(this).Handle;Native.Rect rect;Native.GetWindowRect(h,out rect);var screen=Forms.Screen.FromHandle(h);var b=screen.WorkingArea;settings.Monitor=screen.DeviceName;settings.PanelX=Math.Max(0,Math.Min(1,(rect.Left-b.Left)/(double)Math.Max(1,b.Width-(rect.Right-rect.Left))));settings.PanelY=Math.Max(0,Math.Min(1,(rect.Top-b.Top-Reserved)/(double)Math.Max(1,b.Height-Reserved-(rect.Bottom-rect.Top))));settings.PanelFree=true;Position();Save?.Invoke();}
        public void Position(){if(positioning)return;positioning=true;try{var screen=Forms.Screen.AllScreens.FirstOrDefault(s=>s.DeviceName==settings.Monitor)??Forms.Screen.PrimaryScreen;var b=screen.WorkingArea;var h=new WindowInteropHelper(this).Handle;if(h==IntPtr.Zero){Left=b.Left+16;Top=b.Top+16+Reserved;return;}Native.Rect size;Native.GetWindowRect(h,out size);int width=size.Right-size.Left,height=Math.Min(size.Bottom-size.Top,Math.Max(1,b.Height-Reserved-32));int x=PanelPlacement.Position(b.Left,b.Width,width,0,settings.PanelFree,settings.PanelX,!settings.Corner.Contains("izquierda"));int y=PanelPlacement.Position(b.Top,b.Height,height,Reserved,settings.PanelFree,settings.PanelY,!settings.Corner.StartsWith("Superior"));Native.SetWindowPos(h,new IntPtr(-1),x,y,width,height,0x10);}finally{positioning=false;}}

    }
    sealed class HighlightWindow:Window {
        public HighlightWindow(){WindowStyle=WindowStyle.None;AllowsTransparency=true;Background=Brushes.Transparent;ShowInTaskbar=false;ShowActivated=false;Topmost=true;IsHitTestVisible=false;ResizeMode=ResizeMode.NoResize;Content=new Border{BorderBrush=Brushes.DeepSkyBlue,BorderThickness=new Thickness(3),CornerRadius=new CornerRadius(3)};SourceInitialized+=(s,e)=>{var h=new WindowInteropHelper(this).Handle;Native.SetWindowLongPtr(h,-20,new IntPtr(Native.GetWindowLongPtr(h,-20).ToInt64()|0x08000000|0x20));};}
        public void ShowRect(Rect rect){if(rect.IsEmpty||rect.Width<=0||rect.Height<=0)return;Show();Native.SetWindowPos(new WindowInteropHelper(this).Handle,new IntPtr(-1),(int)rect.X,(int)rect.Y,(int)rect.Width,(int)rect.Height,0x10);}
    }
}
