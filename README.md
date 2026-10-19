<p align="center"><img src="assets/brand/logo.png" alt="Logo de Adapta PE Lite" width="180"></p>

# Adapta PE Lite para Windows

## Comandos naturales: bloque del 17 de octubre de 2026

La palabra predeterminada admite «Computadora», «Computadoras» y «Computador». Una llamada seguida sólo de cortesía mantiene la espera de ocho segundos. Las palabras personales siguen siendo independientes: elegir «Asistente» no activa esos alias. Se conserva el texto original tras la llamada, incluidos espacios interiores y Unicode; las pruebas de texto no acreditan reconocimiento con micrófono.

El parser acepta cortesía y variantes españolas como «por favor abre Safari», «quiero que abras Safari», «ábrame Safari» y «abrí Safari». Conserva mayúsculas, acentos y espacios interiores del dictado. La selección literal y las consultas a la API conservan su puntuación. Se mantienen los atajos nativos existentes.

Las partículas finales ambiguas «pe», «pues», «ya pe» y «ya pues» sólo se retiran si llevan una coma antes. Así «busca Adapta PE» y «busca qué significa pues» conservan su contenido. «Abre Safari, pe» se prepara como «abre Safari». Es una diferencia conservadora respecto a la extensión de referencia; no se adivina si una palabra final pertenece a un nombre o consulta.

Este bloque porta reglas de texto españolas. No añade reconocimiento acústico en seis idiomas ni acredita precisión con micrófonos reales.

Lite conserva sus exclusiones de cámara, cinética, TalkBack automático e IA. Las comprobaciones portables y la compilación cruzada no acreditan voz, filtros, WPF ni instalación en Windows real.

Edición **Lite 1.0 independiente en C# y WPF**, derivada de Adapta-PE-Win 1.0.1. Incluye dos funciones: **asistente de voz completo y siete filtros de daltonismo**, con bandeja, panel flotante, preferencias y ayuda. Conserva los principios de las versiones Lite de macOS y extensión web. Usa el logo Lite proporcionado por el autor en el ejecutable, ventanas, bandeja e instalador.

No incluye cámara, cursor cinético, clic por permanencia, TalkBack automático ni IA externa. Esos motores, modelos de visión y dependencias OpenCV no existen en este proyecto ni se compilan. La **lectura manual bajo una orden explícita** pertenece al asistente y se conserva.

**Estado:** `net10.0-windows`, `net48` y `net461` compilados desde macOS; instalador NSIS generado y **343 comprobaciones** aprobadas sobre lógica y paquete publicado. El instalador pesa **95,3 MiB**, un **36,3 % menos** que el instalador Base 1.0.1 de referencia. Es una comparación de archivos, no de CPU/RAM. Resultados en [VALIDACION-WINDOWS.md](docs/VALIDACION-WINDOWS.md). Falta acreditar ejecución real de Lite, voz, filtros e instalación en Windows. El uso confirmado de Base no valida automáticamente Lite. No hay release pública.

## Mascotas y mejoras compartidas · 11 de octubre de 2026

Se contrastaron las extensiones completas 13.0 y Lite 3.0, sus README y los motores actuales. Las mejoras se adaptan a las APIs del escritorio; cada edición conserva sus funciones.

En **Mascota**, elige **Tinku** (gallito), **Cuchu** (cuy), **Yati** (llama) o **Sin mascota**, que es el valor inicial. Sólo se decodifica el atlas elegido. Los cuadros nativos de 96×112 conservan transparencia, secuencias de entrada/salida y duración de los sprites originales. Un temporizador sigue la duración de los cuadros; otro se programa una sola vez para el descanso. No se repinta toda la interfaz. La animación se detiene al dormir, con movimiento reducido y durante suspensión. Ocultar/cambiar libera el atlas anterior.

La mascota mira hacia el cursor cercano, acompaña la llamada al asistente y cambia entre hablar y leer durante la salida de voz real. **Saludar**, **Dormir** y **Despertar** permiten controlar su estado; dormir se mantiene hasta despertar. El dibujo deja pasar los clics. Arrastra su nombre entre pantallas o pulsa **Mover con teclas**, usa las flechas y vuelve con **Escape**. **Otro monitor** cambia la pantalla sin arrastrar; se recuerdan mascota, monitor y posición relativa.

### Interfaz, panel y compañía

Las funciones principales quedan en secciones con nombres directos. **Usar panel** abre PiP; **Opciones** permite elegir mascota, filtros, esquina y monitor. Los dispositivos y preferencias de reconocimiento permanecen bajo sus opciones avanzadas. Lite mantiene voz, filtros y lectura manual por orden; no añade cámara, cursor, TalkBack automático ni IA. **Detener todo** permanece accesible.

Activa **Mover mascota y panel juntos** para situar la mascota arriba de PiP. Arrastra el título del panel o el nombre de la mascota para mover ambos. Al separarlos, la mascota recupera su posición propia. Se recuerdan unión, monitor y posición relativa del panel; cambiar de esquina vuelve al anclaje. El tamaño se limita al área útil reservando lugar para la mascota. Windows usa la barra de título nativa; Ajustes y Detener todo permanecen en un pie fijo al desplazar el contenido.

La llamada por voz usa el teléfono una vez; ejecutar una orden usa la laptop una vez. Leer y hablar acompañan únicamente la salida de voz real. Después vuelve a reposo, sin dejar teléfono/laptop trabajando indefinidamente. Tras **90 segundos sin interacción**, descansa y detiene su animación. La próxima acción despierta el descanso automático; **Dormir** manual exige **Despertar**. No duerme automáticamente durante una lectura o respuesta hablada.

Di **«computadora, une mascota al panel»**, **«separa mascota del panel»** o **«panel esquina superior izquierda»**. También se admite **«mueve el panel a la esquina inferior derecha»**. Cambiar posición usa los mismos límites que el arrastre y conserva el control disponible.

Después de llamar al asistente, admite **«mascota Cu Chu»**, **«mascota Tinku»**, **«mascota Yaty»**, **«elige mascota tiiinku»**, **«oculta mascota»**, **«saluda mascota»**, **«duerme mascota»** y **«despierta mascota»**. Las órdenes se reconocen completas; mencionar una mascota dentro de dictado o búsqueda conserva ese texto.

**Lite continúa independiente:** voz, lectura manual solicitada, siete filtros y mascota. No se añaden cámara, cursor cinético, TalkBack automático ni IA.

### Pantalla completa y varios monitores

En Windows 8 o posterior, [MagSetFullscreenColorEffect](https://learn.microsoft.com/en-us/windows/win32/api/magnification/nf-magnification-magsetfullscreencoloreffect) aplica la matriz a todo el escritorio, sin capturar ni copiar cada monitor. Mantiene restauración del efecto previo, comprobación de propietario y recuperación. Windows 7 conserva la capa a 10 FPS, ahora ajustada al cambiar las pantallas; no se acredita compatibilidad con juegos exclusivos en ese sistema.

La mascota es una ventana superior y la entrada usa el escritorio virtual, con coordenadas negativas. Las acciones por voz dependen de los permisos, foco y controles publicados por cada app. Usa **ventana sin bordes/pantalla completa de escritorio** para juegos que no admitan capas en modo exclusivo. Algunos juegos con entrada propia o protección antitrampas pueden rechazar entrada sintética; no se eluden esas restricciones ni UAC.

### Código y comprobación

- [`Core/Mascot.cs`](src/AdaptaPELite/Core/Mascot.cs): atlas, secuencias y comandos independientes de la UI.
- [`Windows/Mascot.cs`](src/AdaptaPELite/Windows/Mascot.cs): carga sólo del recurso elegido, dos ventanas, voz, posición y suspensión.
- [`assets/mascotas/`](assets/mascotas/): sprites locales y manifiestos derivados de las extensiones.

Se compilaron `net10.0-windows`, `net48` y `net461` desde macOS. Las 372 comprobaciones portables aprobaron comandos, estados/recursos de las tres mascotas, separación Lite y preferencias propias. No se ejecutó WPF en Windows ni se midieron micrófono, DPI mixto o FPS reales. El aviso NU1900 del entorno impidió consultar el catálogo de vulnerabilidades de NuGet; no se cambiaron paquetes.

## Voz y comandos: versión 1.0

Cambio de reconocimiento, micrófono o palabra de activación conserva la escucha si estaba activa. Espera a que el motor anterior libere audio/modelo; varios cambios rápidos conservan sólo el último. Detener todo durante esa espera impide el arranque pendiente. Cambiar ajustes con voz apagada no la activa.

SAPI combina dictado libre con una gramática finita de comandos, palabra de activación y nombres del catálogo instalado. Prefiere un reconocedor español de la cultura del usuario cuando existe. El motor local mantiene su modelo español y dictado libre; no se ha medido una mejora de precisión acústica con micrófonos reales. Un resultado SAPI de baja confianza muestra diagnóstico y no ejecuta la orden.

El parser admite más de cien frases adicionales y cortesía como «oye», «me puedes», «podrías», «quiero que», «necesito» y «hazme el favor de». Ejemplos:

- «Computadora, me puedes cerrar la app Google por favor»: Google/Chrome/crom se resuelven al proceso Chrome.
- «Computadora, cambia a Discord», «trae Chrome al frente», «muestra apps abiertas»: busca ventanas abiertas sin depender de un acceso directo. Abrir una app ya abierta enfoca su ventana; varias ventanas o coincidencias aproximadas requieren elección.
- «Computadora, hazme un clic derecho», «abre el menú contextual», «doble clic»: clic en la aplicación de destino bajo el cursor.
- «Computadora, léeme la pantalla», «leer la pantalla», «qué dice la pantalla», «detén la lectura»: lectura manual del contenido accesible de la ventana externa. No añade OCR ni promete leer contenido que UIA no publica.
- «Computadora, guarda el documento», «reabre la pestaña», «muestra descargas», «ve al final», «maximiza la ventana».
- «Computadora, apaga la voz»: detiene escucha; se reactiva mediante ajustes o PiP.

Catálogo de Inicio más App Paths de HKCU/HKLM en ambas vistas del registro. Sólo abre ejecutables/accesos existentes; no acepta argumentos shell dictados. Chrome puede detectarse por registro aunque no tenga acceso de Inicio. «Abre la página de Google» elige explícitamente el sitio; «abre Google» puede enfocar/abrir Chrome si está disponible. Se conservan las 28 rutas de sitios.

Nombres parecidos exigen elegir incluso si hay una sola posibilidad. Cierre conserva HWND/PID de la ventana enumerada, los revalida y solicita WM_CLOSE; no termina procesos. Un título de página que menciona Word no acredita que Chrome sea Word. Dictado conserva Unicode y texto literal; frases negadas no activan acciones por aproximación.

Validación de esta actualización: comprobaciones portables y compilación cruzada desde macOS. Reconocimiento hablado, cambio de motor, ventanas reales, UIA y ejecución del instalador siguen pendientes en Windows.

## Instalación y actualización

En Windows 10/11 **x64**, ejecuta **`artifacts/AdaptaPELite-1.0-Setup-x64.exe`**. Instala para tu usuario en `%LOCALAPPDATA%\Programs\AdaptaPELite`, incluye .NET y el reconocimiento español local, y permite elegir accesos en escritorio e Inicio. No necesitas SDK, extensión, Python, servidor ni cuenta.

El asistente muestra Windows detectado, ubicación y versión. Ofrece **Instalar**, **Actualizar** o **Reparar** según la instalación encontrada. Bloquea sustituir una versión más reciente. Conserva preferencias y elección de accesos. Cierra Lite desde su bandeja antes de actualizar o desinstalar. Las actualizaciones son manuales; no hay servicio ni consulta de red al iniciar. Lite omite el instalador Visual C++ usado por cámara en Base: las DLL de voz incluidas utilizan dependencias Windows/GCC comprobadas y no requieren ese componente.

Base y Lite tienen ejecutables, accesos, carpetas, preferencias, inicio automático y registros de desinstalación separados. Instalar Lite no actualiza Base. Los filtros comparten un mutex para evitar que una edición deshaga el efecto de la otra. Conviene activar voz y filtros en una sola edición a la vez.

Desinstala desde Configuración de Windows o el desinstalador de su carpeta. Se eliminan sólo archivos conocidos y accesos propios. Preferencias en `%LOCALAPPDATA%\AdaptaPELite` y archivos ajenos se conservan. Los runtimes compartidos del sistema no se eliminan.

El instalador carece de firma Authenticode; Windows puede mostrar editor desconocido. No desactives SmartScreen, antivirus ni UAC. El instalador moderno no se usa en Windows 7/8/8.1 ni ARM64: esos sistemas requieren paquetes separados y pruebas.

Para uso portable, conserva **toda la carpeta publicada** y ejecuta `AdaptaPELite.exe`. Un EXE aislado sin sus DLL y `models/speech/` no es el paquete completo. Las publicaciones modernas no autocontenidas necesitan [.NET Desktop Runtime 10](https://learn.microsoft.com/en-us/dotnet/core/install/windows).

## Primer uso

1. El primer lanzamiento deja voz, filtros e inicio con Windows apagados.
2. Enfoca una aplicación externa, como Bloc de notas. En **Asistente de voz**, activa Voz y di **«Computadora, muestra campos»**.
3. En **Dispositivos y reconocimiento**, elige micrófono, motor y palabra de activación. La señal del micrófono indica entrada de audio.
4. En **Daltonismo**, elige un filtro. **Sin filtro** o **Quitar filtro** restaura el efecto anterior cuando sigue siendo el controlado por Adapta.
5. Abre **Panel flotante · PiP** o cierra ajustes. La asistencia continúa y permanece accesible desde bandeja.
6. **Detener todo** cancela acciones y elecciones, apaga micrófono y lectura, retira el filtro propio y recuerda asistencia apagada. **Ctrl+Shift+S** realiza la misma parada si Windows permite registrar el atajo.

**Salir** conserva preferencias para otro inicio. Los módulos recordados se intentan restaurar y los fallos se muestran. Bloqueo y suspensión detienen entrada/micrófono/filtros; al volver se comprueba escritorio de usuario antes de reanudar. Inicio con Windows es voluntario desde **Ayuda**, nunca desde el instalador.

## Asistente y comandos

Precede cada orden con «Computadora» o el nombre elegido. Decir sólo el nombre emite una señal y abre **ocho segundos** para dar la orden. Durante lectura se admiten interrupciones explícitas. Puedes escribir una orden desde ajustes después de enfocar el destino externo.

- **Aplicaciones y sitios:** «abre Bloc de notas», «abre You Tube en nueva ventana», «abre Visual Studio Code en el monitor dos», «busca movilidad en Wikipedia». Catálogo de apps local y 28 sitios/alias de Base.
- **Cerrar por nombre:** «cierra Chrome», «cerrar la aplicación Word», «ciérrame la app Bloc de notas». Prioriza identidad del proceso, revalida ventana/PID y solicita elección ante varias ventanas o nombres aproximados. No fuerza terminación ni omite guardar cambios.
- **Navegación y teclado:** «nueva pestaña», «regresa», «refresca la página», «cambia a la pestaña anterior», «guardar», «imprimir», «mostrar escritorio», «presiona control shift N».
- **Campos y controles:** «muestra campos», «enfoca Nombre», «escribe Hola, José! en el campo Nombre», «selecciona Contacto», «pulsa lo seleccionado», «selecciona el texto Buenos días», «borra el campo», «clic», «clic derecho».
- **Elecciones:** «elige dos», «la segunda», número, nombre único, «siguiente», «anterior», «cancelar». Hasta seis por página; cambian/caducan con el destino y tras 45 segundos. Una elección pendiente no exige repetir activación.
- **Lectura manual y menús:** «lee la pantalla», «lee el botón Contacto», «muestra comandos», «ejecuta la acción Exportar PDF», «silencio». La respuesta se sintetiza localmente.
- **Filtros:** «filtro deuteranopía», «quitar filtro». Los nombres se comparan sin distinguir acentos/mayúsculas.
- **Aplicación:** «mostrar ajustes», «modo flotante», «panel esquina superior derecha», «detener todo», «salir de adapta».

Las órdenes cinéticas, TalkBack e IA de Base responden que la función no está disponible. No se convierten en acciones sobre menús externos. «Clic derecho» inmediato sí pertenece al asistente; «siguiente clic derecho» y «cancelar clic» por permanencia no están disponibles.

UI Automation depende de los controles publicados por cada aplicación. Campos protegidos quedan excluidos de lectura, selección y dictado. El dictado conserva Unicode, signos y mayúsculas recibidos del motor; Vosk suele producir texto sin capitalización/puntuación automática. No se ejecutan scripts ni comandos shell dictados.

Se utilizan atajos Windows y foco comprobado. Las URLs sólo admiten HTTP/HTTPS. El navegador enfocado se usa cuando resulta compatible; la búsqueda contextual puede volver a Google. No se promete equivalencia con DOM o `chrome.tabs`. UIPI impide controlar algunas aplicaciones elevadas, UAC y escritorio seguro. No necesitas ejecutar toda Adapta como administrador.

## Voz local y diagnóstico

| Ajuste | Comportamiento |
| --- | --- |
| Nombre del asistente | `Computadora`; personalizable, de 1 a 40 caracteres |
| Automático | SAPI español instalado; fallback al reconocimiento local incluido |
| Local incluido | Vosk 0.3.45 y modelo español pequeño, cargados bajo demanda |
| SAPI | Necesita reconocedor español y dictado; una voz TTS no basta |
| Micrófono | Predeterminado de Windows o dispositivo seleccionado |
| Inicio con Windows | Apagado; optativo en Ayuda |

Una captura PCM mono de **16 kHz** por sesión. WinMM usa `CALLBACK_EVENT`, buffers en orden y almacenamiento acotado. Detener invalida callbacks anteriores. Automático cambia una vez al motor local si SAPI termina inesperadamente; no reinicia indefinidamente un reconocedor terminado. Audio y transcripciones no se guardan ni envían a una API.

Si no escucha, revisa permiso del micrófono, dispositivo y señal. Prueba **Local incluido**. Si detecta la orden pero falla la acción, enfoca el destino y revisa su respuesta. Una confirmación «Orden detectada» no acredita que Windows permita ejecutarla. Si la salida TTS falla, la respuesta escrita permanece disponible.

## Daltonismo

Protanomalía, protanopía, deuteranomalía, deuteranopía, tritanomalía, tritanopía y acromatopsia. Conserva exactamente las matrices de Base y sus equivalentes web/macOS; son **simulaciones visuales**, no una corrección médica.

Windows 8+ usa efecto global con [Magnification API](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/magapi/magapi-intro); no hay captura continua de pantalla para ese efecto. Windows 7 usa una capa de magnificación a 10 FPS, todavía pendiente de pruebas reales. El proceso debe coincidir con arquitectura nativa; x86 bajo Windows x64 no sirve para estos filtros.

Adapta consulta el efecto anterior, verifica si sigue controlando el actual y evita sobrescribir cambios de otras ayudas al restaurar. Un archivo local permite intentar recuperación tras un cierre abrupto en el siguiente inicio. Si falla la retirada, el error queda visible y puedes reintentar **Detener todo**. La coexistencia con otras ayudas requiere prueba en Windows.

## Panel y consumo

PiP conserva interruptor de Voz, transcripción, respuesta/error, elecciones, Ajustes y Detener todo. Puedes arrastrar su barra de título o elegir esquina/monitor. Guarda la posición relativa, reserva espacio para la mascota unida y conserva Ajustes/Detener todo en un pie fijo. No toma foco al aparecer. Sus controles usan el mismo motor de voz que ajustes. La bandeja permite cambiar voz/filtros, mostrar panel o salir.

La UI conserva fondo Base, logo Lite, estilos WPF, foco visible, controles de al menos 44 píxeles lógicos y navegación de teclado. Hay dos páginas funcionales más Ayuda, sin controles de cámara o IA ocultos.

No hay OpenCV, modelos ONNX, inferencia, preview, timer de cursor, lectura automática bajo cursor ni red en reposo. Imágenes de marca congeladas y compartidas. SAPI usa recursos del sistema; Vosk carga el modelo sólo al activar ese motor y lo libera al apagar. El [catálogo Vosk](https://alphacephei.com/vosk/models) orienta consumo de modelos pequeños; no representa una medición de Lite. Bandeja y eventos de ventana/sesión permanecen disponibles con asistencia apagada.

## Compatibilidad

| Target | Destino y límite |
| --- | --- |
| `net10.0-windows`, x64 | Principal para Windows 11 y prioridad de ejecución Windows 10; instalador con runtime incluido |
| `net48`, x64 | Legacy para Windows 7 SP1, 8.1 y cuando corresponda Windows 10; necesita .NET Framework 4.8 |
| `net461`, x64 | Windows 8.0 histórico; necesita .NET Framework 4.6.1, sin soporte vigente |
| x86 | Compilación opcional para Windows x86; voz SAPI, sin Vosk x64; filtros no admiten WOW64 |
| ARM64 | No distribuido ni validado |

La matriz oficial de [.NET para Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows) limita soporte Windows 10 a ediciones LTSC/Enterprise admitidas; no se declara soporte oficial .NET 10 en Home/Pro 22H2. Las compilaciones Legacy no prueban funcionamiento físico en sistemas históricos. Consulta también [requisitos de .NET Framework](https://learn.microsoft.com/en-us/dotnet/framework/get-started/system-requirements). Windows 7/8/8.1 y .NET Framework 4.6.1 no tienen soporte vigente.

## Construir desde el repositorio

Requisitos: SDK .NET 10, NuGet y, para instalador, **NSIS 3.13**. El usuario final recibe un paquete autocontenido; no necesita SDK. Código fuente: [TheAlejo160/Adapta-PE-Lite-Win](https://github.com/TheAlejo160/Adapta-PE-Lite-Win). Los instaladores de `artifacts/` se generan localmente y no se incluyen al clonar. La publicación de fuentes no acredita ejecución Windows ni una release instalable validada.

Desde la raíz, en PowerShell:

```powershell
./scripts/checks.ps1
./scripts/build.ps1
./scripts/build.ps1 -SelfContained
./scripts/build.ps1 -Framework net48
./scripts/build.ps1 -Framework net461
./scripts/installer.ps1 -MakeNsis "C:/Program Files (x86)/NSIS/makensis.exe"
```

`build.ps1` produce `artifacts/<framework>-<runtime>/`. `installer.ps1` publica en `artifacts/net10.0-windows-win-x64-contained/`, conserva licencias del runtime y genera la lista específica de desinstalación. El instalador queda en `artifacts/AdaptaPELite-1.0-Setup-x64.exe`. No hay instalador Legacy acreditado. Los permisos y dispositivos no se activan durante compilación.

También puedes usar directamente:

```powershell
dotnet build src/AdaptaPELite/AdaptaPELite.csproj -c Release
dotnet run --project tests/AdaptaPELite.Checks -c Release
dotnet publish src/AdaptaPELite/AdaptaPELite.csproj -f net10.0-windows -c Release -r win-x64 --self-contained true -o artifacts/net10.0-windows-win-x64-contained
dotnet run --project tests/AdaptaPELite.Checks -c Release -- artifacts/net10.0-windows-win-x64-contained
# Sólo Windows: abre ajustes y deja la aplicación abierta.
./scripts/check-startup.ps1 -Executable ./artifacts/net10.0-windows-win-x64-contained/AdaptaPELite.exe
# Sólo Windows: exige 30 segundos de escucha y apaga Voz al terminar.
./scripts/check-voice.ps1
```

Las comprobaciones cubren parser, alias, cierre por proceso, dictado Unicode, navegación, activación y TTS, contrato de audio continuo, resultados Vosk, matrices e índices inválidos, identidad/preferencias Lite y rechazo de funciones excluidas. Con carpeta publicada inspeccionan los tipos del ensamblado sin ejecutar WPF, el modelo español y ausencia de OpenCV/ONNX. No activan sensores ni usan servicios.

```text
src/AdaptaPELite/
  App.cs, MainWindow.cs, Theme.cs   WPF, bandeja, panel y ciclo de vida
  Core/                           Parser, sitios, alias, activación, audio y matrices
  Windows/                        SAPI/Vosk/WinMM, UIA manual, Win32, filtros y preferencias
assets/brand/                     Logo Lite original, ICO y fondo Base
assets/models/speech/             Modelo español y manifiesto de procedencia/hashes
assets/speech-runtime/            Vosk x64 y licencias
assets/installer/                 BMP Lite y licencias NSIS
scripts/                         Compilación, instalador y comprobaciones Windows
tests/AdaptaPELite.Checks/         Lógica portable y exclusiones de ensamblado/paquete
```

El código es independiente: no depende de que Base permanezca en una carpeta hermana. Conserva los motores existentes con recortes específicos, sin nuevas abstracciones, servidores ni frameworks visuales. [AGENTS.md](AGENTS.md) replica los contratos de Base adaptados al alcance Lite; [memoria de código](docs/MEMORIA-CODIGO.md) describe el índice y sus límites.

### Clonar, compilar e instalar desde PowerShell

En Windows 10/11 x64 con WinGet disponible, instala Git y el **SDK** .NET 10 si faltan. Abre otra terminal después de instalarlos para actualizar el PATH. Si WinGet no está disponible, utiliza los instaladores oficiales de Git y [.NET](https://learn.microsoft.com/en-us/dotnet/core/install/windows).

```powershell
winget install --id Git.Git --exact
winget install --id Microsoft.DotNet.SDK.10 --exact
```

```powershell
git clone https://github.com/TheAlejo160/Adapta-PE-Lite-Win.git
Set-Location Adapta-PE-Lite-Win
dotnet --info
dotnet restore src/AdaptaPELite/AdaptaPELite.csproj
./scripts/checks.ps1
./scripts/build.ps1 -Framework net10.0-windows -Runtime win-x64 -SelfContained
# Ejecutar la publicación portable en Windows; conservar toda la carpeta.
& ./artifacts/net10.0-windows-win-x64/AdaptaPELite.exe
```

Para instalar con accesos directos y desinstalador, cierra la app desde la bandeja, instala **NSIS 3.13** y genera el instalador. Puedes comprobar su disponibilidad con `winget show --id NSIS.NSIS --exact --versions` y usar `winget install --id NSIS.NSIS --exact --version 3.13`, o descargarlo desde [NSIS](https://nsis.sourceforge.io/Download). La compilación exige `makensis` en PATH o su ruta explícita.

Lite no utiliza el prerrequisito Visual C++ de cámara de Base. Conserva las DLL y el modelo de voz incluidos.

```powershell
./scripts/installer.ps1 -MakeNsis 'C:/Program Files (x86)/NSIS/makensis.exe'
Start-Process ./artifacts/AdaptaPELite-1.0-Setup-x64.exe -Wait
# Después de completar el asistente de instalación:
& "$env:LOCALAPPDATA/Programs/AdaptaPELite/AdaptaPELite.exe"
```

Para actualizar las fuentes, usa `git pull --ff-only`, vuelve a compilar y ejecuta el nuevo instalador. Conserva tus cambios locales antes de actualizar. El instalador admite reparación y bloquea una revisión técnica anterior. Para desinstalar, cierra Adapta y usa Configuración de Windows o el `Uninstall.exe` de su carpeta de instalación; se conservan preferencias personales.

Estos comandos describen el flujo Windows. En macOS sólo se puede compilar de forma cruzada y ejecutar las comprobaciones portables; no se ejecutan WPF, Win32 ni el instalador. Legacy se construye con `-Framework net48` o `-Framework net461` según la tabla de compatibilidad y necesita su runtime instalado, sin `-SelfContained`.

## Privacidad y autoría

Preferencias con reemplazo seguro en `%LOCALAPPDATA%\AdaptaPELite\settings.json`; recuperación de filtro en la misma carpeta. No guarda grabaciones, transcripciones, capturas ni claves. No incluye integración IA ni telemetría remota. Las búsquedas web comparten la consulta con el sitio como navegación normal. No sustituye todas las funciones de Narrador/NVDA.

© 2026 **Rodrigo Alejandro Apcho Aliaga — TheAlejo160**. Se conservan [LICENSE](LICENSE), [LICENSE.txt](LICENSE.txt) y [avisos de terceros](THIRD_PARTY_NOTICES.md). El proyecto mantiene Creative Commons Atribución-NoComercial-CompartirIgual 4.0; componentes de terceros conservan sus licencias. [Licencia oficial](https://creativecommons.org/licenses/by-nc-sa/4.0/deed.es).

Se utilizaron herramientas de inteligencia artificial generativa para asistencia en código, documentación y recursos gráficos del proyecto, bajo diseño, integración y supervisión del autor. El logo Lite recibido se conserva íntegro; su empaquetado no redibuja la imagen.

Comprobaciones de esta entrega: 335 sin sensores y 340 con inspección del paquete. Compilación cruzada de `net10.0-windows`, `net48` y `net461`; Una consulta inicial de vulnerabilidades NuGet no estuvo disponible; la compilación final termina sin errores ni advertencias. Instalador local x64 generado; ejecución e instalación Windows pendientes.

## Cómo se organiza y ejecuta el código

| Archivo o módulo | Responsabilidad |
| --- | --- |
| [App.cs](src/AdaptaPELite/App.cs) | Entrada WPF, instancia única y cierre. |
| [MainWindow.cs](src/AdaptaPELite/MainWindow.cs) | Compone los motores, sincroniza Ajustes/PiP y ejecuta Detener todo. |
| [Core/Commands.cs](src/AdaptaPELite/Core/Commands.cs) | Intenciones y frases finitas; normaliza comandos y conserva dictado literal. |
| [Core/RecognitionSession.cs](src/AdaptaPELite/Core/RecognitionSession.cs) | Ordena cambios de voz y descarta inicios/callbacks antiguos. |
| [Windows/Voice.cs](src/AdaptaPELite/Windows/Voice.cs) | Selecciona SAPI/Vosk y coordina escucha, TTS y recuperación. |
| [Windows/Actions.cs](src/AdaptaPELite/Windows/Actions.cs) | Ejecuta intenciones contra una aplicación externa y comunica el resultado. |
| [Windows/DesktopAccess.cs](src/AdaptaPELite/Windows/DesktopAccess.cs) | Consulta UI Automation, campos, selección y lectura manual con destino verificado. |
| `Core/` restante | Catálogo de sitios, alias, activación y matrices. |
| `Windows/` restante | Entrada Win32, aplicaciones, filtros, preferencias y capacidades del sistema. |
| [scripts/installer.ps1](scripts/installer.ps1) y [scripts/installer.nsi](scripts/installer.nsi) | Publican un paquete autocontenido y generan instalación/desinstalación por usuario. |

Una orden pasa por **captura → reconocimiento → activación «Computadora» → parser → Actions → UIA/Win32 → respuesta**. `MainWindow.Run` muestra la intención detectada; sólo la respuesta posterior acredita la acción. El destino externo se conserva aunque se abra PiP. Las opciones ambiguas se cancelan si cambia la aplicación; las contraseñas se excluyen.

Cambiar micrófono/motor invalida la sesión anterior y espera su liberación. **Detener todo** invalida arranques pendientes, cancela acciones/elecciones, detiene escucha y TTS, retira el filtro propio y recuerda módulos apagados. **Salir** libera recursos conservando preferencias.

El [mapa de código](docs/MEMORIA-CODIGO.md) amplía los contratos y límites del índice. El grafo es una ayuda de navegación; las fuentes, manifiestos y NSIS son la referencia para verificar el comportamiento. `.codebase-memory/` es memoria local regenerable y se excluye de Git. Las pruebas, licencias y recursos necesarios sí se publican.

## Numeración unificada

Por indicación del autor, Base y Lite usan la versión pública **1.0** en el producto, interfaz, panel, instalador, Configuración de Windows y nombre del archivo. La interfaz lee la versión informativa del ensamblado; no mantiene etiquetas duplicadas independientes.

La revisión técnica de archivo **1.0.3.0** ordena las actualizaciones sin cambiar la versión pública. El instalador guarda `DisplayVersion=1.0` y `BuildVersion=1.0.3.0`; compara la revisión para actualizar/reparar y bloquear paquetes anteriores. Acepta las instalaciones previas que sólo registraron `DisplayVersion` o tienen un EXE en la ubicación propia. Se conserva la protección frente a una revisión superior, las preferencias y los accesos elegidos.

Compilación y empaquetado desde macOS; la migración del registro y ejecución del instalador requieren Windows real.

Verificación de la numeración unificada: 343 comprobaciones aprobadas sobre el paquete, incluida versión de producto 1.0 y revisión de archivo 1.0.3.0. Compilación cruzada sin errores ni advertencias en los tres targets; fuentes NSIS y migración de claves revisadas directamente. Instalación/actualización, WPF y reconocimiento con micrófono permanecen pendientes en Windows.

## Verificación de la documentación

El 3 de octubre de 2026 se revisaron README, recursos y exclusiones de Git. Pasaron 335 comprobaciones portables y 343 al inspeccionar el paquete local, incluidas las exclusiones Lite. No se ejecutaron WPF, micrófono ni instalador en Windows durante esta revisión documental.

## Proyectos de la familia Adapta PE

| Programa | Código y alcance |
| --- | --- |
| Extensión Base | [Adapta-PE](https://github.com/TheAlejo160/Adapta-PE): voz, filtros, cursor de página y TalkBack. |
| Extensión Lite | [Adapta-PE-Lite](https://github.com/TheAlejo160/Adapta-PE-Lite): voz y filtros. |
| Windows Base | [Adapta-PE-Win](https://github.com/TheAlejo160/Adapta-PE-Win): aplicación WPF independiente. |
| Windows Lite | [Adapta-PE-Lite-Win](https://github.com/TheAlejo160/Adapta-PE-Lite-Win): voz, lectura manual y filtros. |
| macOS completo | [Adapta-PE-MacOS-Intel-Apple-Silicon-](https://github.com/TheAlejo160/Adapta-PE-MacOS-Intel-Apple-Silicon-): sólo Adapta PE completo, con su proyecto Xcode. Lite tiene repositorio y fuentes independientes. |
| macOS Lite independiente | [Adapta-PE-Lite-MacOS](https://github.com/TheAlejo160/Adapta-PE-Lite-MacOS): voz y siete filtros en una aplicación autónoma. |

Los números de versión y permisos pertenecen a cada plataforma. Un ZIP de la extensión no instala la aplicación de escritorio. Los repositorios publican fuentes y recursos necesarios; los instaladores generados y la configuración personal quedan fuera de Git.

### Paquetes locales de prueba: 17 de octubre de 2026

Se recompiló net10.0-windows para win-x64 autocontenido y se generó el instalador NSIS con los recursos y licencias actuales. IncludeSourceRevisionInInformationalVersion=false conserva ProductVersion exactamente 1.0, sin sufijo Git, como exige el check de paquete; FileVersion sigue siendo 1.0.3.0. Se verificaron hashes de recursos, checks portables, integridad del archivo NSIS y correspondencia de la DLL incluida con la recién compilada. Esto no acredita instalación/desinstalación, WPF, sensores o voz en Windows. No tiene firma Authenticode.
