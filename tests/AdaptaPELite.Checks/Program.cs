using System;
using System.Linq;
using AdaptaPE.Core;

static class Program
{
    static int checks;
    sealed class AudioProbe:LiveAudioStream{public override bool CanRead{get{return true;}}public override int Read(byte[] buffer,int offset,int count){Consumed(count);return count;}}
    static void Check(bool valid,string reason){checks++;if(!valid)throw new Exception(reason);}
    static void Main(){
        foreach(var word in new[]{"Computadora","Computadoras","Computador"}){
            bool signal;var activation=new WakeState();
            Check(activation.Consume(word+", por favor","computadora",0,false,false,out signal)==null&&signal,"Cortesía sola conserva espera: "+word);
            Check(activation.Consume("Abre Safari","computadora",7.99,false,false,out signal)=="Abre Safari","Espera disponible antes de ocho segundos");
            activation.Consume(word,"computadora",10,false,false,out signal);
            Check(activation.Consume("Abre Safari","computadora",18,false,false,out signal)==null,"Espera vence a los ocho segundos");
            Check(activation.Consume("oye   "+word+" escribe Hola,  Perú!","computadora",20,false,false,out signal)=="escribe Hola,  Perú!","Espacios antes de activación no recortan contenido");
        }
        foreach(var rejected in new[]{"compu","ordenador","supercomputadora","computadoramente"}){bool signal;Check(new WakeState().Consume(rejected+" abre Safari","computadora",0,false,false,out signal)==null&&!signal,"Rechaza activación parcial: "+rejected);}
        bool customSignal;var customWake=new WakeState();
        var invalidClock=new WakeState();invalidClock.Consume("Computadora","computadora",0,false,false,out customSignal);
        Check(invalidClock.Consume("Computadora abre Safari","computadora",double.NaN,false,false,out customSignal)==null&&invalidClock.Consume("abre Safari","computadora",1,false,false,out customSignal)==null,"Reloj inválido cancela la espera de activación");
        Check(customWake.Consume("oye   MA\u0301QUINA   AZUL escribe Hola,  Perú!","Máquina Azul",0,false,false,out customSignal)=="escribe Hola,  Perú!","Activación personal Unicode conserva índices originales");
        Check(new WakeState().Consume("Computadora abre Safari","Asistente",0,false,false,out customSignal)==null,"Palabra personal no activa aliases predeterminados");
        foreach(var phrase in new[]{"ábrame Safari","abrí Safari","¿puedes abrir Safari?","podrías abrirme Safari por favor","quiero que abras Safari","me haces el favor de abrir Safari","ayúdame a abrir Safari","oye, porfa abre Safari, pe","oye, ¿puedes abrir Safari?"}){
            var prepared=Commands.Parse(phrase);Check(prepared.Kind=="open"&&prepared.Text=="Safari","Cortesía y conjugaciones: "+phrase);
        }
        Check(Commands.Parse("escribí Ya pe, abre YouTube.").Text=="Ya pe, abre YouTube.","Dictado conserva muletillas y signos");
        Check(Commands.Parse("escribe Hola,  José!").Text=="Hola,  José!","Dictado conserva espacios");
        Check(Commands.Parse("oye, búscame Ají de Gallina en YouTube porfa").Text=="Ají de Gallina","Búsqueda conserva consulta");
        Check(Commands.Parse("por favor no abras Safari").Kind=="unknown","Cortesía no oculta negación");
        foreach(var phrase in new[]{"¿puedes abrir Safari?","escribí Ya pe, abre YouTube.","oye, búscame Ají de Gallina en YouTube porfa","no abras Safari","buscar texto"})Check(Commands.Prepare(Commands.Prepare(phrase))==Commands.Prepare(phrase),"Preparación idempotente: "+phrase);
        Check(Commands.Parse("escribi\u0301 Hola, José!").Text=="Hola, José!","Conjugación con acento descompuesto");
        Check(Commands.Parse("seleccione el texto Hola, José!").Text=="Hola, José!","Selección literal conserva puntuación");
        Check(Commands.Parse("por favor quitar filtro").Kind=="filter"&&Commands.Parse("por favor quitar filtro").Text=="ninguno","Cortesía conserva retirada de filtro");
        Check(Commands.Parse("por favor ya no hables!").Kind=="silence","Cortesía conserva interrupción");
        Check(Commands.Parse("para la lectura").Kind=="silence","«Para la lectura» interrumpe");
        foreach(var off in new[]{"desactiva voz","apaga el micrófono"}){var parsed=Commands.Parse(off);Check(parsed.Kind=="voice"&&parsed.Text=="off","Apaga voz: "+off);bool offBeep;Check(new WakeState().Consume("Computadora, "+off,"Computadora",0,true,false,out offBeep)==off,"Apagar voz llega al ejecutor durante la lectura: "+off);}
        bool readingBeep;Check(new WakeState().Consume("Computadora, abre Chrome","Computadora",0,true,false,out readingBeep)==null,"Durante la lectura no se ejecutan otras órdenes");
        Check(Commands.Parse("pon el texto Hola, por favor.").Text=="Hola, por favor.","Prefijo de dictado histórico conserva signos");
        Check(Commands.Parse("abrir adapta").Kind=="app"&&Commands.Parse("abrir adapta").Text=="abrir adapta","Apertura del panel conserva su acción");
        Check(Commands.Parse("borrar campo").Kind=="clear","Borrado de campo conserva su acción");
        Check(Commands.Parse("busca Adapta PE").Text=="Adapta PE","Búsqueda conserva siglas finales");
        Check(Commands.Parse("busca qué significa pues").Text=="qué significa pues","Búsqueda conserva palabra final literal");
        Check(Commands.Parse("abre Adapta PE").Text=="Adapta PE","Apertura conserva nombre completo");
        Check(Commands.Prepare("oye, ¿puedes abrir Safari?")==Commands.Prepare(Commands.Prepare("oye, ¿puedes abrir Safari?")),"Puntuación interna conserva idempotencia");
        Check(Commands.Parse("oye, ¡detén todo!").Kind=="stop","Puntuación interna no bloquea emergencia");

        foreach(double fraction in new[]{-10.0,0,.5,1,10,double.NaN,double.PositiveInfinity}){
            int x=PanelPlacement.Position(-1920,1920,350,0,true,fraction,false),y=PanelPlacement.Position(-200,1080,600,164,true,fraction,false);Check(x>=-1920&&x+350<=0&&y>=-36&&y+600<=880,"Panel libre y mascota caben con coordenadas negativas");
        }
        Check(PanelPlacement.Position(0,900,600,164,false,0,false)==180&&PanelPlacement.Position(0,900,600,164,false,0,true)==284,"Esquinas reservan espacio para la mascota");
        var activity=new MascotActivity();activity.Touch(10);Check(!activity.ShouldSleep(99)&&activity.ShouldSleep(100),"Descanso sólo tras 90 segundos");activity.Speaking=true;Check(!activity.ShouldSleep(200),"No duerme durante lectura o respuesta");activity.Speaking=false;activity.Touch(200);Check(!activity.ShouldSleep(201),"Interacción reinicia descanso");activity.ManualSleep=true;Check(!activity.ShouldSleep(300),"Sueño manual exige despertar");
        Check(Commands.Parse("une mascota al panel").Text=="join"&&Commands.Parse("separa la mascota del panel").Text=="separate","Unión y separación por voz");Check(Commands.Parse("escribe une mascota al panel").Kind=="dictate","Unión no intercepta dictado");Check(Commands.Parse("mueve el panel a la esquina superior izquierda").Kind=="corner"&&Commands.Parse("panel esquina inferior derecha").Text=="inferior derecha","Posición del panel por voz");
        foreach(var pair in new[]{Tuple.Create("mascota Cu Chu","cuchu"),Tuple.Create("elige mascota tiiinku","tinku"),Tuple.Create("mascota Yaty","yati"),Tuple.Create("oculta la mascota","ninguna"),Tuple.Create("duerme mascota","sleep")})Check(Commands.Parse(pair.Item1).Kind=="mascot"&&Commands.Parse(pair.Item1).Text==pair.Item2,"Comandos de mascota: "+pair.Item1);
        Check(Commands.Parse("escribe mascota Cuchu").Kind=="dictate"&&Commands.Parse("escribe mascota Cuchu").Text=="mascota Cuchu","Mascota no altera dictado");
        Check(Commands.Parse("busca mascota Yati en Google").Kind=="search","Mascota no altera búsqueda");
        foreach(string name in new[]{"tinku","cuchu","yati"}){
            MascotAtlas atlas;using(var file=System.IO.File.OpenRead("assets/mascotas/mascota-"+name+".json"))atlas=(MascotAtlas)new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(MascotAtlas)).ReadObject(file);
            Check(atlas.Valid(1178,1370),"Atlas completo y referencias válidas: "+name);var animation=new MascotPlayback(atlas);animation.Set("wave");for(int i=0;i<100;i++)animation.Advance();Check(animation.State=="idle","Saludo termina en reposo");animation.Set("laptop");for(int i=0;i<100;i++)animation.Advance();Check(animation.State=="idle","Acción termina en reposo");animation.Set("sleep");for(int i=0;i<100;i++)animation.Advance();Check(animation.State=="sleep"&&animation.Resting,"Dormir termina animación y conserva sueño");animation.Set("wake");for(int i=0;i<100;i++)animation.Advance();Check(animation.State=="idle","Despertar termina en reposo");
        }

        var command=Commands.Parse("Computadora");Check(command.Kind=="unknown","Palabra sola no es una acción de escritorio");
        foreach(var phrase in new[]{"cierra Chrome","cerrar la aplicación Google Chrome.","por favor cierra el navegador crom","ciérrame la app Chrome","sierra Chrome","sal de Chrome","puedes cerrar Chrome por favor","por favor puedes cerrar Chrome","cierra por favor Chrome"}){
            command=Commands.Parse(phrase);Check(command.Kind=="closeApp"&&AppNames.Canonical(command.Text)=="google chrome","Cierre admite artículos, imperativo, puntuación y variantes de reconocimiento");
        }
        foreach(var pair in new[]{Tuple.Create("Bloc de notas","notepad"),Tuple.Create("Word","WINWORD"),Tuple.Create("Chrome","chrome"),Tuple.Create("Edge","msedge"),Tuple.Create("Calculadora","CalculatorApp"),Tuple.Create("VS Code","Code")})Check(AppNames.RunningRank(pair.Item1,pair.Item2,"Documento") == 3,"Identifica aplicación por proceso, no por título de documento");
        Check(AppNames.RunningRank("Word","chrome","Página — Microsoft Word")==0,"No cierra navegador por un título que menciona otra app");
        Check(AppNames.RunningRank("Word","wordpad","Microsoft Word")==0,"No confunde un proceso diferente con Word");
        Check(AppNames.RunningRank("Calculadora","ApplicationFrameHost","Calculadora")==2,"Reconoce ventana UWP con proceso contenedor");
        Check(Commands.Parse("detén todo").Kind=="stop","Parada de emergencia en imperativo");
        Check(Commands.Parse("cerrar la ventana").Kind=="shortcut"&&Commands.Parse("cerrar la pestaña").Text=="CTRL+W","Distingue cierre actual de cierre por nombre");
        foreach(var pair in new[]{Tuple.Create("abre una nueva pestaña","CTRL+T"),Tuple.Create("cerrar esta ventana actual","ALT+F4"),Tuple.Create("refresca la página por favor.","CTRL+R"),Tuple.Create("regresa","ALT+LEFT"),Tuple.Create("cambia a la pestaña anterior","CTRL+SHIFT+TAB")})Check(Commands.Parse(pair.Item1).Kind=="shortcut"&&Commands.Parse(pair.Item1).Text==pair.Item2,"Navegación natural equivalente a extensión");
        Check(Commands.Parse("escribe Ayúdame, por favor!").Text=="Ayúdame, por favor!","Cortesía forma parte del dictado literal");
        Check(Commands.Parse("no hagas clic derecho").Kind=="unknown","No ejecutar clic derecho desde frases negadas");
        command=Commands.Parse("Escribe Hola, José! en el campo Nombre");Check(command.Kind=="writeField"&&command.Text=="Hola, José!"&&command.Target=="Nombre","Dictado conserva Unicode y signos");
        command=Commands.Parse("busca arriba y abajo en Wikipedia");Check(command.Kind=="search"&&command.Text=="arriba y abajo"&&command.Target=="wikipedia","Búsqueda no se interpreta como scroll");
        command=Commands.Parse("abre You Tube en nueva ventana");Check(command.Kind=="open"&&command.Text=="You Tube"&&command.Destination=="ventana","Destino de navegación");
        Check(Commands.FindSite("equis").Id=="x"&&Commands.FindSite("Netflix").Id=="netflix","Alias exactos");Check(Commands.Sites.Length==28,"Catálogo de 28 sitios");
        Check(Commands.FindSite("instagram").Query("Hola José").Contains("site%3Awww.instagram.com"),"Búsqueda alternativa codificada");
        Check(KeyNames.TryParse("control shift N").SequenceEqual(new ushort[]{0x11,0x10,0x4E}),"Atajo nativo");Check(KeyNames.TryParse("cmd.exe /c del")==null,"No acepta shell dictado");
        Check(Commands.Ordinal(Commands.Parse("elige dos").Text)==1&&Commands.Ordinal("el sexto")==5,"Elección por números hablados y ordinales");
        bool beep;var wake=new WakeState();Check(wake.Consume("Computadora","Computadora",0,false,false,out beep)==null&&beep,"Activación con señal");Check(wake.Consume("Abre Chrome","Computadora",7,false,false,out beep)=="Abre Chrome","Espera de ocho segundos");
        wake.Consume("Computadora","Computadora",10,false,false,out beep);Check(wake.Consume("abre Chrome","Computadora",19,false,false,out beep)==null,"Activación caducada");Check(wake.Consume("Computadora abre Chrome","Computadora",20,true,false,out beep)==null,"Ignora acciones durante TTS");Check(wake.Consume("Computadora silencio","Computadora",21,true,false,out beep)=="silencio","Interrupción explícita");
        var matrix=ColorMatrices.Native(2);Check(Math.Abs(matrix[0]-.567)<.0001&&Math.Abs(matrix[5]-.433)<.0001&&Math.Abs(matrix[1]-.558)<.0001&&matrix[18]==1&&matrix[24]==1,"Transposición RGB a matriz nativa");
        using(var audio=new AudioProbe()){Check(audio.Length==-1&&!audio.CanSeek&&audio.Position==0,"Contrato SAPI de audio continuo sin longitud finita");Check(audio.Read(new byte[32],0,32)==32,"Captura informa bytes consumidos");Check(audio.Position==32&&audio.Seek(0,System.IO.SeekOrigin.Current)==32,"Posición acumulada permite consulta SAPI");bool rejected=false;try{audio.Position=0;}catch(NotSupportedException){rejected=true;}Check(rejected,"No promete rebobinar audio descartado");}
        foreach(bool partial in new[]{false,true}){var bytes=System.Text.Encoding.UTF8.GetBytes(partial?"{\"partial\":\"computadora\"}\0":"{\"text\":\"Hola, José\"}\0");var pointer=System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length);try{System.Runtime.InteropServices.Marshal.Copy(bytes,0,pointer,bytes.Length);Check(AdaptaPE.Windows.LocalSpeech.Text(pointer,partial)==(partial?"computadora":"Hola, José"),"Reconocimiento local decodifica finales y provisionales UTF-8");}finally{System.Runtime.InteropServices.Marshal.FreeHGlobal(pointer);}}
        foreach(var phrase in new[]{"control cabeza","control rostro","control muñón","centrar cursor","calibrar cursor","activa el cursor","pausa el cursor","movilidad reducida","movimientos pequeños","activar temblor","descansar","continuar cursor","cursor más rápido","modo preciso","activar clic automático","siguiente clic derecho","cancelar clic","activar talkback","pregunta a la IA ¿Qué es Perú?","analiza pantalla con IA"})
            Check(Commands.Parse(phrase).Kind=="unsupported","Lite rechaza motores excluidos: "+phrase);
        Check(Commands.Parse("clic derecho").Kind=="rightClick","Clic derecho inmediato sigue perteneciendo al asistente");
        Check(Commands.Parse("lee la pantalla").Kind=="readScreen","Lectura manual sigue perteneciendo al asistente");
        Check(Commands.Parse("escribe pregunta a la IA Hola").Text=="pregunta a la IA Hola","Texto dictado no se bloquea por mencionar una función");
        Check(Commands.Parse("abre ChatGPT").Kind=="open","Abrir un sitio no activa IA externa");
        Check(ColorMatrices.Names.Length==8&&ColorMatrices.Titles.Length==8,"Siete filtros y opción sin filtro");
        foreach(int index in Enumerable.Range(0,8)){var m=ColorMatrices.Native(index);Check(m.Length==25&&m[18]==1&&m[24]==1&&m.All(x=>!float.IsNaN(x)&&!float.IsInfinity(x)),"Matrices completas y alfa conservado");}
        foreach(int index in new[]{-1,8}){bool rejected=false;try{ColorMatrices.Native(index);}catch(ArgumentOutOfRangeException){rejected=true;}Check(rejected,"Rechaza índices de filtro inválidos");}
        wake.Consume("Computadora","Computadora",30,false,false,out beep);wake.Cancel();Check(wake.Consume("abre Chrome","Computadora",31,false,false,out beep)==null,"Detener cancela activación pendiente");
        Check(wake.Consume("elige dos","Computadora",32,false,true,out beep)=="elige dos","Elecciones no exigen repetir activación");
        Check(!new AdaptaPE.Windows.Preferences().VoiceActive&&!new AdaptaPE.Windows.Preferences().Login,"Primer inicio y arranque automático apagados");
        Check(AdaptaPE.Windows.Preferences.Folder.EndsWith("AdaptaPELite"),"Preferencias separadas de Base");
        var preferenceNames=typeof(AdaptaPE.Windows.Preferences).GetFields().Where(f=>Attribute.IsDefined(f,typeof(System.Runtime.Serialization.DataMemberAttribute))).Select(f=>f.Name).ToArray();
        Check(!preferenceNames.Any(n=>new[]{"Camera","Mode","Fps","Tremor","DwellEnabled","CameraActive","TalkBackActive","AIEnabled","Endpoint","Model"}.Contains(n)),"Preferencias excluyen módulos Base");
        if(Environment.GetCommandLineArgs().Length>1)CheckPackage(Environment.GetCommandLineArgs()[1]);
        foreach(var pair in Commands.NaturalShortcuts)Check(Commands.Parse(pair.Key).Kind=="shortcut"&&Commands.Parse(pair.Key).Text==pair.Value,"Atajo natural: "+pair.Key);
        foreach(var pair in Commands.NaturalActions)Check(Commands.Parse(pair.Key).Kind==pair.Value,"Acción natural: "+pair.Key);
        foreach(var prefix in new[]{"","por favor ","oye, ","me puedes ","podrías ","quiero que ","necesito ","hazme el favor de "})
        foreach(var name in new[]{"Chrome","Google","crom","google crom","Google Chrome"}){
            var close=Commands.Parse(prefix+"cerrar la app "+name+" por favor.");Check(close.Kind=="closeApp"&&AppNames.Canonical(close.Text)=="google chrome","Cierre coloquial: "+prefix+name);
            Check(AppNames.RunningRank(close.Text,"chrome","Documento — Google Chrome")==3,"Chrome por proceso incluso con alias coloquial");
        }
        foreach(var name in new[]{"Teams","Discord","Telegram","Zoom","VLC","OBS","Steam","Firefox","Brave","Recortes","Configuración"})
            Check(AppNames.MatchRank(name,AppNames.Canonical(name))==3,"Catálogo amplía alias: "+name);
        Check(AppNames.RunningRank("mi navegador","chrome","Documento")==1,"Navegador genérico exige elección");
        Check(Commands.Parse("abre la página de https://example.org/RutaMayúscula").Text=="https://example.org/RutaMayúscula","URL explícita conserva ruta y mayúsculas");
        Check(Commands.FindSite(AppNames.Clean(Commands.Parse("abre la página de Google").Text)).Id=="google","Sitio explícito conserva resolución real");
        Check(AppNames.RunningRank("chrom","chrome","Word — Google Chrome")==1,"Error aproximado de Chrome exige elección");
        Check(AppNames.RunningRank("Chrome","msedge","Google Chrome")==0,"Otro navegador con título Chrome nunca es Chrome");
        Check(AppNames.MatchRank("","Chrome")==0&&AppNames.MatchRank("cro","Microsoft Word")==0,"Nombres vacíos/cortos no producen parecidos");
        Check(Commands.Parse("cambia a Chrome").Kind=="switchApp"&&Commands.Parse("trae Chrome al frente").Text=="chrome","Enfocar app abierta");
        Check(Commands.Parse("abre la página de Google").Target=="site","Sitio explícito no se confunde con aplicación");
        Check(Commands.Parse("puedes escribir No cierres Chrome, por favor!").Text=="No cierres Chrome, por favor!","Dictado literal conserva cortesía y negación");
        foreach(var phrase in new[]{"no cierres Chrome","nunca cierres Chrome","no hagas un clic derecho","quiero que no leas la pantalla"})Check(Commands.Parse(phrase).Kind=="unknown","Negación no activa comando: "+phrase);
        Check(Commands.Parse("busca cómo cerrar Chrome en Google").Kind=="search","Consulta no ejecuta intención contenida");
        Check(Commands.SpeechPhrases.All(p=>Commands.Parse(p).Kind!="unknown"),"Gramática finita corresponde al parser: "+string.Join(", ",Commands.SpeechPhrases.Where(p=>Commands.Parse(p).Kind=="unknown")));
        Check(KeyNames.TryParse("PAGEUP")[0]==0x21&&KeyNames.TryParse("PAGEDOWN")[0]==0x22,"Teclas de avance de página Windows");
        var session=new RecognitionSession();var release=new System.Threading.Tasks.TaskCompletionSource<bool>();int starts=0;
        var firstStart=session.StartAfter(release.Task,session.Begin(),()=>starts++);
        Check(starts==0,"No abre un segundo reconocedor mientras se libera el anterior");
        var secondStart=session.StartAfter(release.Task,session.Begin(),()=>starts+=10);
        release.SetResult(true);System.Threading.Tasks.Task.WhenAll(firstStart,secondStart).GetAwaiter().GetResult();
        Check(starts==10,"Sólo el último cambio de motor inicia reconocimiento");
        release=new System.Threading.Tasks.TaskCompletionSource<bool>();var stoppedStart=session.StartAfter(release.Task,session.Begin(),()=>starts++);
        session.Cancel();release.SetResult(true);stoppedStart.GetAwaiter().GetResult();Check(starts==10,"Detener durante liberación impide reactivación tardía");
        Console.WriteLine(checks+" comprobaciones aprobadas. No validan ejecución Windows, micrófono ni rendimiento.");
    }
    static void CheckPackage(string folder){
        var version=System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(folder,"AdaptaPELite.dll"));
        Check(version.ProductVersion=="1.0","Producto publicado muestra exactamente 1.0");
        Check(version.FileVersion=="1.0.3.0","Revisión de archivo conserva orden de actualización");
        Check(new Version(version.FileVersion)>new Version("1.0.2.0"),"Actualiza desde Base 1.0.2 y Lite 1.0.1 sin bajar la revisión");
        Check(System.IO.File.Exists(System.IO.Path.Combine(folder,"AdaptaPELite.exe")),"Ejecutable Lite publicado");
        var files=System.IO.Directory.GetFiles(folder,"*",System.IO.SearchOption.AllDirectories);
        Check(!files.Any(f=>f.EndsWith(".onnx",StringComparison.OrdinalIgnoreCase)||System.IO.Path.GetFileName(f).IndexOf("opencv",StringComparison.OrdinalIgnoreCase)>=0),"Paquete sin modelos de visión ni OpenCV");
        using(var stream=System.IO.File.OpenRead(System.IO.Path.Combine(folder,"AdaptaPELite.dll")))
        using(var pe=new System.Reflection.PortableExecutable.PEReader(stream)){
            var metadata=System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
            var names=metadata.TypeDefinitions.Select(h=>metadata.GetString(metadata.GetTypeDefinition(h).Name)).ToArray();
            Check(names.Contains("Voice")&&names.Contains("Filters"),"Paquete contiene motores de voz y filtros");
            Check(!names.Any(n=>new[]{"Camera","LocalVision","KineticState","OptionalAI","DwellWindow"}.Contains(n)),"Motores excluidos no se compilan");
        }
        Check(System.IO.File.Exists(System.IO.Path.Combine(folder,"models","speech","es","am","final.mdl")),"Modelo español incluido");
    }
}
