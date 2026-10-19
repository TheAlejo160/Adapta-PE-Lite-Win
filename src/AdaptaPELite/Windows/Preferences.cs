using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Win32;

namespace AdaptaPE.Windows
{
    [DataContract]
    public sealed class Preferences
    {
        [DataMember] public string WakeWord="Computadora",Filter="ninguno",Corner="Inferior derecha",Monitor="";
        [DataMember] public string Mascot="ninguna",MascotMonitor="";
        [DataMember] public double MascotX=.75,MascotY=.8,PanelX,PanelY;
        [DataMember] public bool MascotLinked,PanelFree;

        [DataMember] public int Microphone=-1;
        [DataMember] public string SpeechEngine="auto";
        [DataMember] public bool VoiceActive=false,Floating=false,Login=false;
        public static readonly string Folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"AdaptaPELite");
        static readonly string FilePath=Path.Combine(Folder,"settings.json");
        public static string Notice="";
        public static Preferences Load() {
            try{if(!File.Exists(FilePath))return new Preferences();using(var s=File.OpenRead(FilePath)) {
                var p=(Preferences)new DataContractJsonSerializer(typeof(Preferences)).ReadObject(s);
                if(p==null)throw new InvalidDataException();
                if(!AdaptaPE.Core.MascotCommand.Names.Contains(p.Mascot))p.Mascot="ninguna";
                if(double.IsNaN(p.MascotX)||double.IsInfinity(p.MascotX)||p.MascotX<0||p.MascotX>1)p.MascotX=.75;
                if(double.IsNaN(p.MascotY)||double.IsInfinity(p.MascotY)||p.MascotY<0||p.MascotY>1)p.MascotY=.8;
                if(double.IsNaN(p.PanelX)||double.IsInfinity(p.PanelX)||p.PanelX<0||p.PanelX>1)p.PanelX=.5;
                if(double.IsNaN(p.PanelY)||double.IsInfinity(p.PanelY)||p.PanelY<0||p.PanelY>1)p.PanelY=.5;
                p.Monitor=p.Monitor??"";p.MascotMonitor=p.MascotMonitor??"";
                if(string.IsNullOrWhiteSpace(p.WakeWord))p.WakeWord="Computadora";
                if(p.WakeWord.Length>40)p.WakeWord="Computadora";
                if(p.Microphone < -1)p.Microphone=-1;
                if(p.SpeechEngine!="auto"&&p.SpeechEngine!="local"&&p.SpeechEngine!="sapi")p.SpeechEngine="auto";
                if(Array.IndexOf(AdaptaPE.Core.ColorMatrices.Names,p.Filter)<0)p.Filter="ninguno";
                if(p.Corner!="Superior izquierda"&&p.Corner!="Superior derecha"&&p.Corner!="Inferior izquierda"&&p.Corner!="Inferior derecha")p.Corner="Inferior derecha";
                return p;
            }}catch(Exception e){if(!(e is IOException||e is SerializationException||e is ArgumentException||e is UnauthorizedAccessException))throw;Notice="Configuración dañada o inaccesible; se usan valores seguros.";return new Preferences();}
        }
        public void Save() {
            Directory.CreateDirectory(Folder);string temporary=FilePath+".tmp";
            using(var s=new FileStream(temporary,FileMode.Create,FileAccess.Write,FileShare.None)){new DataContractJsonSerializer(typeof(Preferences)).WriteObject(s,this);s.Flush(true);}
            if(File.Exists(FilePath))File.Replace(temporary,FilePath,null);else File.Move(temporary,FilePath);
        }
#if NET10_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
        public static void SetLogin(bool enabled,string executable) {
            using(var key=Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
                if(enabled)key.SetValue("AdaptaPELite","\""+executable+"\" --background");else key.DeleteValue("AdaptaPELite",false);
            }
        }
    }
}
