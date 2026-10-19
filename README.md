# Adapta PE Lite para Windows

<p align="center"><img src="assets/brand/logo.png" alt="Logo de Adapta PE Lite" width="180"></p>

Adapta PE Lite es una aplicación de escritorio independiente en C# y WPF para manejar aplicaciones por voz, dictar, leer contenido accesible bajo una orden explícita y aplicar siete filtros de color. Incluye mascotas opcionales, panel flotante y bandeja del sistema.

Está pensada para quien quiere estas ayudas en un PC modesto o sin cámara. Frente a [Windows Base](https://github.com/TheAlejo160/Adapta-PE-Win), Lite elimina cámara, OpenCV, modelos de visión, cursor cinético, clic por permanencia, TalkBack automático e IA externa. Conserva el asistente completo, las acciones UI Automation y la lectura manual. Tiene menos dependencias; el ahorro real de CPU/RAM sigue sin medirse en hardware Windows. El modelo de voz Vosk sí consume memoria cuando está activo.

La versión pública es **1.0**; la revisión técnica del archivo es `1.0.3.0`. Este repositorio contiene fuentes y recursos: los instaladores de `artifacts/` se generan localmente y no se incluyen al clonar. La compilación cruzada y las comprobaciones portables registradas en [validación Windows](docs/VALIDACION-WINDOWS.md) no acreditan ejecución de WPF, voz, filtros ni instalación en Windows real.

## Elegir la compilación adecuada

El [proyecto](src/AdaptaPELite/AdaptaPELite.csproj) declara tres targets y arquitectura x64 por defecto:

| Compilación | Windows de destino | Runtime y límites |
| --- | --- | --- |
| `net10.0-windows`, x64 | Windows 11; Windows 10 como objetivo de pruebas | .NET 10. El instalador moderno incluye el runtime. |
| `net48`, x64 | Windows 7 SP1 y 8.1; vía Legacy para Windows 10 cuando corresponda | Requiere .NET Framework 4.8 instalado. Paquete separado. |
| `net461`, x64 | Windows 8.0 histórico | Requiere .NET Framework 4.6.1, sin soporte vigente. Paquete separado. |

La [matriz oficial de .NET en Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows) limita el soporte de Windows 10 a ediciones LTSC/Enterprise admitidas. No se declara soporte oficial de .NET 10 en Windows 10 Home/Pro 22H2. Revisa también los [requisitos de .NET Framework](https://learn.microsoft.com/en-us/dotnet/framework/get-started/system-requirements). La compatibilidad histórica no equivale a soporte vigente del sistema o del runtime.

El instalador actual es sólo para Windows 10/11 x64. No sirve para Windows 7/8/8.1 ni acredita ARM64. El script permite compilar x86 para Windows x86, usando SAPI y sin Vosk x64; los filtros no admiten un proceso x86 bajo WOW64. ARM64 no está distribuido ni validado. Los targets Legacy compilados siguen pendientes de pruebas en sus sistemas reales.

## Instalar un paquete local

Si ya dispones del paquete generado para Windows 10/11 x64:

1. Cierra Lite desde el menú de bandeja **Salir** si está abierta.
2. Ejecuta `AdaptaPELite-1.0-Setup-x64.exe`.
3. Revisa la ubicación y elige los accesos en escritorio e Inicio que necesites.
4. Abre Adapta PE Lite y sigue el tutorial de abajo.

Instala para tu usuario en `%LOCALAPPDATA%\Programs\AdaptaPELite`. Incluye .NET y reconocimiento español Vosk local: no necesitas SDK, extensión, Python, servidor ni cuenta. No incluye el prerrequisito Visual C++ de cámara de Base. Conserva todas las DLL y el modelo de voz del paquete.

El asistente ofrece instalación, actualización o reparación según la instalación encontrada. La revisión técnica bloquea downgrades aunque la versión pública siga siendo 1.0. Las actualizaciones son manuales y conservan preferencias y elección de accesos; no hay servicio de actualización ni consulta de red al iniciar.

El instalador local documentado no tiene firma Authenticode. SmartScreen puede mostrar editor desconocido o una advertencia de reputación: comprueba procedencia y, si se proporciona, el hash del archivo antes de decidir si continuar. Una detección de malware o una política del equipo requiere resolver ese aviso concreto. No desactives SmartScreen, antivirus ni UAC.

Para uso portable, conserva **toda la carpeta publicada** y abre `AdaptaPELite.exe`. Copiar sólo el EXE deja fuera DLL, recursos y `models/speech/`. Una publicación moderna sin runtime incluido requiere [.NET Desktop Runtime 10](https://learn.microsoft.com/en-us/dotnet/core/install/windows), que es el runtime para WPF.

Desinstala desde Configuración de Windows o `Uninstall.exe` en la carpeta de instalación, después de salir de Lite. Se borran archivos conocidos y accesos propios; se conservan preferencias en `%LOCALAPPDATA%\AdaptaPELite`, archivos ajenos y runtimes compartidos. Base y Lite tienen ejecutables, instalación, preferencias y registros separados: instalar Lite no actualiza Base. Sus filtros comparten exclusión para no deshacer el efecto de la otra edición; utiliza la asistencia de una edición a la vez.

## Compilar y ejecutar desde este repositorio

Haz estos pasos en Windows, desde PowerShell. Necesitas Git y el **SDK .NET 10**, no sólo el runtime. La última compilación cruzada descrita en la documentación usó SDK **10.0.402**; no hay un `global.json` que fije ese parche. Descarga el SDK de la [página oficial de .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). La restauración necesita acceso a NuGet o una caché completa.

```powershell
git clone https://github.com/TheAlejo160/Adapta-PE-Lite-Win.git
Set-Location Adapta-PE-Lite-Win
dotnet --info
dotnet restore src/AdaptaPELite/AdaptaPELite.csproj
dotnet build src/AdaptaPELite/AdaptaPELite.csproj -c Release -f net10.0-windows
dotnet run --project src/AdaptaPELite/AdaptaPELite.csproj -c Release -f net10.0-windows
```

`dotnet run` abre la app de escritorio. Para Visual Studio, Microsoft indica [Visual Studio 2026 18.0 o posterior para .NET 10](https://learn.microsoft.com/en-us/dotnet/core/install/windows#net-versions-and-visual-studio). Instala la carga de trabajo de desarrollo de escritorio con .NET y abre `src/AdaptaPELite/AdaptaPELite.csproj`. La terminal permite seguir el flujo sin instalar el IDE.

### Publicar una carpeta portable

```powershell
./scripts/build.ps1 -Framework net10.0-windows -Runtime win-x64 -SelfContained
& ./artifacts/net10.0-windows-win-x64/AdaptaPELite.exe
```

`build.ps1` escribe en `artifacts/<framework>-<runtime>/`, también con `-SelfContained`. Sin ese modificador, la publicación moderna necesita Desktop Runtime 10 en el equipo de destino. Para generar las compilaciones históricas, desde un equipo de desarrollo compatible:

```powershell
./scripts/build.ps1 -Framework net48 -Runtime win-x64
./scripts/build.ps1 -Framework net461 -Runtime win-x64
```

Legacy necesita su .NET Framework instalado en destino y no acepta `-SelfContained`. Estas carpetas no son un instalador Legacy validado.

### Crear el instalador por usuario

Instala NSIS 3.13 desde [NSIS](https://nsis.sourceforge.io/Download) y cierra Lite antes de probar el resultado:

```powershell
./scripts/installer.ps1 -MakeNsis 'C:/Program Files (x86)/NSIS/makensis.exe'
Start-Process ./artifacts/AdaptaPELite-1.0-Setup-x64.exe -Wait
```

Si `makensis` está en PATH, basta `./scripts/installer.ps1`. El script comprueba los hashes del manifiesto de voz, publica una carpeta autocontenida en `artifacts/net10.0-windows-win-x64-contained/`, ejecuta los checks del paquete, conserva licencias y genera una lista de borrado específica para NSIS. El instalador no habilita inicio con Windows ni eleva la app al terminar.

Para actualizar las fuentes, conserva tus cambios locales, ejecuta `git pull --ff-only` y repite compilación y empaquetado. No publiques un instalador como validado sin las pruebas Windows pendientes.

## Tutorial: de la primera orden al uso diario

### Activar voz y comprobar el micrófono

El primer inicio deja voz, filtro e inicio con Windows apagados. Abre **Asistente de voz** y despliega **Dispositivos y reconocimiento**. Allí están **Micrófono**, **Reconocimiento de voz** y **Nombre para llamar al asistente**.

1. Elige el micrófono; puedes usar **Dispositivos de sonido** para revisar la entrada de Windows.
2. Pulsa **Permiso del micrófono**. En Windows 11 revisa Configuración → Privacidad y seguridad → Micrófono; en Windows 10, Configuración → Privacidad → Micrófono. Permite el acceso al micrófono para aplicaciones de escritorio si la política del equipo lo permite.
3. Marca **Activar voz** y espera a que el estado indique SAPI o voz local lista. **Señal del micrófono** muestra si llega audio.
4. Di «Computadora, abre Bloc de notas». Después, enfoca su campo de texto y di «Computadora, escribe Hola, José».

Puedes decir sólo «Computadora»: una señal abre ocho segundos para dar la orden. La palabra predeterminada también admite «Computadoras» y «Computador». Si eliges un nombre personal, de 1 a 40 caracteres, usa ese nombre; no conserva los alias predeterminados. Cambiar motor, micrófono o palabra mantiene la intención de escucha y espera a liberar el motor anterior. Cambiarlos con voz apagada no la activa.

Para probar una acción sin reconocimiento, enfoca primero el destino externo, vuelve a ajustes y despliega **Escribir una orden**. Escribe la orden sin activación y pulsa **Ejecutar orden** o Enter. La app conserva el destino externo.

### Elegir entre SAPI y Vosk

| Opción de Reconocimiento de voz | Qué utiliza |
| --- | --- |
| Automático · SAPI o local incluido | Prefiere un reconocedor SAPI español instalado; permite fallback a Vosk local. |
| Local incluido · español | Vosk 0.3.45 y modelo español pequeño, cargados bajo demanda. |
| SAPI instalado en Windows | Requiere reconocedor español con dictado; muestra el error si no está disponible. |

SAPI enumera los reconocedores reales y prefiere la cultura española del usuario cuando existe; no presupone `es-PE`. Revisa las opciones de idioma/voz de Windows si quieres utilizar SAPI. Instalar una voz de síntesis o un paquete de idioma no garantiza que aparezca un reconocedor SAPI compatible con dictado. Si falta, elige **Local incluido · español**.

La síntesis de respuestas usa voces instaladas en Windows y prefiere una española. Reconocimiento y síntesis son componentes distintos. Si falla la salida hablada, permanece la respuesta escrita. El modelo Vosk se libera al apagarlo; suele producir texto sin puntuación ni capitalización automática. Las reglas españolas del parser no equivalen a reconocimiento acústico en seis idiomas.

### Abrir aplicaciones, sitios y búsquedas

Precede estas órdenes con «Computadora» o el nombre elegido:

| Objetivo | Ejemplos |
| --- | --- |
| Abrir una app | «abre Bloc de notas», «por favor abre Chrome» |
| Cambiar de ventana | «muestra apps abiertas», «cambia a Discord», «trae Chrome al frente» |
| Cerrar una app | «cierra Bloc de notas», «me puedes cerrar la app Google por favor» |
| Abrir un sitio explícito | «abre la página de Google», «abre YouTube en nueva pestaña» |
| Buscar | «busca movilidad en Wikipedia», «busca accesibilidad en YouTube» |
| Navegar | «nueva pestaña», «reabre la pestaña», «muestra descargas», «recarga», «atrás» |
| Atajos Windows | «guarda el documento», «copiar», «pegar», «muestra el escritorio», «presiona control shift N» |

El catálogo de aplicaciones combina Inicio, App Paths y ventanas abiertas. Abrir una app ya abierta puede enfocar su ventana. Google/Chrome/crom identifican Chrome para aplicaciones; «abre la página de Google» pide el sitio de forma explícita. El cierre comprueba ventana y proceso y solicita `WM_CLOSE`; no fuerza la terminación ni evita la confirmación de documentos sin guardar.

El [catálogo de sitios](src/AdaptaPELite/Core/Commands.cs) conserva 28 entradas: Google, YouTube, Mercado Libre, Amazon, Wikipedia, Facebook, Instagram, X/Twitter, Reddit, Netflix, Twitch, GitHub, ChatGPT, Claude, Gemini, Canvas, Gmail, Outlook, WhatsApp, Telegram, Drive, Documentos de Google, Mapas, LinkedIn, TikTok, Spotify, Bing y DuckDuckGo. Abrir una web de IA no integra su IA en Lite. Las URLs admiten sólo HTTP/HTTPS; las consultas se codifican para navegación web. Según el navegador y destino, la búsqueda contextual puede volver a Google.

### Dictar, elegir y actuar sobre controles

Enfoca una aplicación externa antes de actuar. Prueba con campos sin datos sensibles:

1. Di «Computadora, muestra campos» para localizar campos accesibles.
2. Si aparecen opciones, di «elige dos» o «la segunda», o pulsa una opción del panel. No necesitas repetir activación para una elección pendiente.
3. Di «Computadora, escribe Hola, José! en el campo Nombre» si la aplicación publica un campo con ese nombre.
4. Para un botón, di «Computadora, selecciona Contacto» y después «Computadora, pulsa lo seleccionado». También puedes pedir «enfoca Nombre» o «selecciona el texto Buenos días».

Las opciones se muestran en páginas de hasta seis: «siguiente» y «anterior» cambian de página; «cancelar» abandona la acción. Caducan tras 45 segundos y se cancelan si cambias de aplicación. Los nombres aproximados requieren elección, incluso con una sola posibilidad.

«Escribe» o «dicta» conserva el texto que sigue, incluidos Unicode, signos, mayúsculas y palabras como «por favor» recibidas del reconocedor. El parser admite cortesía y conjugaciones en órdenes, como «quiero que abras Bloc de notas». Las partículas finales ambiguas «pe», «pues», «ya pe» y «ya pues» sólo se retiran con coma: «busca Adapta PE» conserva PE; «abre Bloc de notas, pe» retira la cortesía.

«Clic», «clic derecho» y «doble clic» son acciones inmediatas del asistente; coloca el cursor sobre la aplicación de destino. «Muestra comandos» y «ejecuta la acción Exportar PDF» consultan acciones que la app publique por UI Automation. Su disponibilidad depende de cada programa. Lite no convierte una orden en PowerShell, `cmd.exe`, scripts o argumentos arbitrarios.

### Leer y detener la lectura

Enfoca la ventana que quieres leer y di «Computadora, lee la pantalla» o «Computadora, léeme la pantalla». Para un elemento concreto, prueba «Computadora, lee el botón Contacto».

La lectura usa contenido accesible de la ventana externa mediante UI Automation. No hay OCR ni lectura automática al pasar el cursor. Las contraseñas quedan excluidas de lectura, selección y dictado. La función no sustituye todas las capacidades de Narrador o NVDA.

Durante la lectura, repite la activación: **«Computadora, para la lectura»**, «Computadora, silencio» o «Computadora, detén la lectura». Para apagar la escucha, di **«Computadora, apaga el micrófono»**; después tendrás que reactivarla desde ajustes, panel o bandeja. **Detener todo** cancela acciones y elecciones, apaga micrófono y síntesis, retira el filtro propio y recuerda esos módulos apagados. También está disponible con **Ctrl+Shift+S** si Windows permite registrar ese atajo.

### Aplicar un filtro

Abre **Filtros de color** y elige **Filtro de pantalla**, o di «Computadora, filtro deuteranopía». Están disponibles protanomalía, protanopía, deuteranomalía, deuteranopía, tritanomalía, tritanopía y acromatopsia. **Quitar filtro**, **Sin filtro** o «Computadora, quitar filtro» retiran el efecto propio.

Las siete matrices son simulaciones visuales, no una corrección médica del daltonismo. Windows 8 o posterior usa un efecto global de Magnification API para el escritorio y todos los monitores, sin captura continua para ese efecto. Windows 7 utiliza una capa de magnificación a 10 FPS pendiente de pruebas reales; no se acredita funcionamiento en juegos exclusivos.

Al restaurar, Lite compara el efecto actual con el que aplicó para evitar sobrescribir cambios de otra ayuda. Intenta recuperar el estado anterior tras un cierre abrupto en el siguiente inicio. Si falla la retirada, reintenta **Detener todo** y revisa el error. La coexistencia con otras ayudas requiere pruebas en Windows.

### Mascota, panel y bandeja

En **Mascota**, elige **Tu mascota**: Tinku (gallito), Cuchu (cuy), Yati (llama) o Sin mascota, que es el valor inicial. Sólo se carga el atlas elegido. Puedes usar **Saludar**, **Dormir**, **Despertar**, **Mover con teclas** y **Otro monitor**. Arrastra su nombre entre pantallas; con el nombre enfocado, usa flechas y Escape. El dibujo deja pasar los clics.

Por voz: «Computadora, mascota Cuchu», «oculta mascota», «saluda mascota», «duerme mascota» y «despierta mascota». Las órdenes completas no interceptan nombres dentro de un dictado o búsqueda. La mascota acompaña activación y órdenes; hablar y leer siguen la síntesis real. Descansa tras 90 segundos sin interacción, sin dormir durante TTS; una acción despierta el descanso automático. Dormir manualmente exige Despertar. Respeta movimiento reducido y suspensión.

Pulsa **Usar panel · PiP**. El panel muestra Voz, transcripción, respuesta, elecciones, **Ajustes** y **Detener todo**. Arrastra su barra de título; en **Ayuda** puedes elegir **Esquina del panel** y **Monitor del panel**. También admite «Computadora, panel esquina superior izquierda». **Mover mascota y panel juntos** coloca la mascota sobre el panel; «une mascota al panel» y «separa mascota del panel» hacen lo mismo por voz. Se recuerda la posición relativa y al separarlos vuelve la posición propia de la mascota.

Cerrar ajustes mantiene la asistencia. El icono de bandeja ofrece **Abrir ajustes**, **Voz: activar / apagar**, **Daltonismo**, **Panel flotante**, **Detener todo** y **Salir**. Salir conserva preferencias para el siguiente arranque; Detener todo recuerda voz y filtro apagados. En **Ayuda**, **Iniciar Adapta PE Lite al entrar en Windows** es voluntario y viene desmarcado. Bloqueo y suspensión detienen asistencia y el retorno comprueba el escritorio de usuario antes de reanudar.

### Si pides una función de Base

«Activa el cursor», «activar TalkBack», «cancelar clic» por permanencia o «pregunta a la IA…» se convierten en `unsupported`. Lite responde: «Adapta PE Lite incluye asistente de voz y filtros. Esa función pertenece a la edición Base». Estas órdenes se resuelven localmente y no pasan al fallback de menús externos.

Considera [Adapta PE para Windows Base](https://github.com/TheAlejo160/Adapta-PE-Win) si necesitas los módulos de cámara/cursor cinético o lectura automática de esa edición. Consulta allí sus requisitos y estado de validación; instalar Base no garantiza que una app o juego admita automatización. Lite conserva voz, lectura manual, filtros y mascotas sin esos módulos.

## Solución de problemas en Windows

| Qué ocurre | Qué revisar |
| --- | --- |
| `dotnet` no aparece o falta el SDK | Instala SDK .NET 10, abre otra terminal y revisa `dotnet --info`. El runtime solo no compila. |
| Falla restauración NuGet | Revisa conexión, proxy o caché. Un aviso de consulta de vulnerabilidades no disponible no acredita una auditoría de paquetes. |
| No escucha o no se mueve la señal | En Dispositivos y reconocimiento revisa micrófono, Permiso del micrófono y Dispositivos de sonido. Comprueba que otra app o política no bloquee la entrada. |
| No hay reconocedor SAPI español | Elige Local incluido · español. Una voz TTS instalada no equivale a reconocimiento con dictado. |
| Vosk no carga o falta una DLL/modelo | Conserva toda la carpeta publicada y `models/speech/`. Usa el paquete x64 completo; no copies DLL de procedencia desconocida. |
| Entiende mal la orden | Espera al estado listo, repite la activación y compara la transcripción. Prueba Escribir una orden para separar reconocimiento de ejecución. |
| Aparece Orden detectada pero no actúa | Lee la respuesta posterior: detectar intención no acredita éxito. Enfoca el destino y comprueba que su control sea accesible y editable. |
| No controla una ventana elevada o UAC | Windows limita entrada por UIPI y escritorio seguro. Usa una app de destino sin elevar cuando sea posible; no desactives UAC ni eleves toda Lite como solución general. |
| No puede leer un control | Puede ser protegido, inaccesible por UIA o no publicar texto. No hay OCR ni acceso a contraseñas. Un proveedor UIA bloqueado puede no responder inmediatamente a cancelar. |
| No se oye la respuesta | Revisa salida de sonido y voces instaladas. La respuesta escrita sigue disponible si falla TTS. |
| Una elección desaparece | Caduca a los 45 segundos o al cambiar la app. Solicita de nuevo los campos/apps y elige una opción vigente. |
| Otro Adapta PE utiliza los filtros | Retira el filtro en la otra edición antes de aplicarlo en Lite. Si otra ayuda cambió el efecto, retira el propio y vuelve a intentarlo. |
| No aparece sobre un juego | Prueba ventana sin bordes/pantalla completa de escritorio. Capas y entrada sintética pueden estar restringidas en modo exclusivo o por antitrampas. No se eluden esas protecciones. |
| Cerré ajustes y sigue escuchando | Es el comportamiento previsto. Usa Detener todo o Salir desde la bandeja. |
| Ctrl+Shift+S está ocupado | Usa Detener todo en panel, ajustes o bandeja; la app muestra el conflicto del atajo. |

## Cómo funciona y qué datos utiliza

El recorrido de una orden es captura local → reconocimiento → activación → parser → acción → respuesta. Estos son los puntos de entrada del código:

| Fuente | Responsabilidad |
| --- | --- |
| [App.cs](src/AdaptaPELite/App.cs) | Entrada WPF, instancia única, cierre y eventos de sesión/pantalla. |
| [MainWindow.cs](src/AdaptaPELite/MainWindow.cs) | Compone voz, acciones, filtros y mascota; sincroniza ajustes, bandeja, PiP y parada. |
| [Core/Commands.cs](src/AdaptaPELite/Core/Commands.cs) | Prepara frases, conserva dictado literal, resuelve sitios/intenciones y rechaza órdenes Base. |
| [Core/RecognitionSession.cs](src/AdaptaPELite/Core/RecognitionSession.cs) | Invalida sesiones anteriores y ordena arranque/liberación de motores. |
| [Windows/Voice.cs](src/AdaptaPELite/Windows/Voice.cs) | SAPI/Vosk, activación y síntesis con voces Windows. |
| [Windows/Actions.cs](src/AdaptaPELite/Windows/Actions.cs) | Fija destino externo, elecciones y ejecución de intenciones. |
| [Windows/DesktopAccess.cs](src/AdaptaPELite/Windows/DesktopAccess.cs) | UI Automation para controles, campos, selección y lectura manual. |
| [Windows/Filters.cs](src/AdaptaPELite/Windows/Filters.cs) | Magnification API, propiedad del efecto, restauración y recuperación. |
| [Core/Mascot.cs](src/AdaptaPELite/Core/Mascot.cs) y [Windows/Mascot.cs](src/AdaptaPELite/Windows/Mascot.cs) | Comandos, secuencias, atlas local y ventanas de la mascota. |

WinMM mantiene una captura PCM mono de 16 kHz por sesión, con buffers acotados y ordenados. Cambiar o detener invalida callbacks anteriores. Automático puede cambiar una vez a Local si SAPI termina inesperadamente; SAPI explícito informa el fallo. Las acciones usan patrones UIA cuando están disponibles y entrada Win32 limitada, revalidando destino/foco. El panel propio no sustituye la aplicación externa de destino. Esto depende de lo que cada programa publique, sin equivalencia garantizada con DOM o `chrome.tabs`.

Preferencias: `%LOCALAPPDATA%\AdaptaPELite\settings.json`, con reemplazo seguro y recuperación ante archivo dañado. La recuperación de filtro se guarda en la misma carpeta. No se guardan grabaciones, transcripciones, capturas ni claves; no hay integración IA externa ni telemetría remota. SAPI instalado o Vosk incluido procesan audio localmente. Las búsquedas y aperturas web sí comparten consulta y navegación con el sitio elegido.

No hay cámara, modelos ONNX, OpenCV, preview ni bucle de cursor/lectura automática. Imágenes de marca compartidas y congeladas; recursos de mascotas en `assets/mascotas/`; voz y procedencia/hashes en `assets/models/speech/` y `assets/speech-runtime/`. El [mapa de código](docs/MEMORIA-CODIGO.md) amplía los contratos y límites del índice. El código funciona sin un repositorio Base en una carpeta hermana.

## Comprobaciones y validación pendiente

Desde la raíz, con SDK .NET 10:

```powershell
./scripts/checks.ps1
# Equivalente directo:
dotnet run --project tests/AdaptaPELite.Checks -c Release
# Inspeccionar además una publicación completa:
dotnet run --project tests/AdaptaPELite.Checks -c Release -- artifacts/net10.0-windows-win-x64-contained
```

La última ruta corresponde a `installer.ps1`; si publicaste con `build.ps1`, usa `artifacts/net10.0-windows-win-x64`. Los checks portables cubren parser, alias, dictado Unicode, activación, sesiones, audio, matrices, mascotas, preferencias y exclusiones Lite. Con carpeta publicada inspeccionan tipos y archivos sin ejecutar WPF, incluido el modelo español y ausencia de dependencias de visión. No usan micrófono ni servicios reales.

Sólo en Windows, después de salir de Lite, puedes comprobar arranque y escucha:

```powershell
./scripts/check-startup.ps1 -Executable ./artifacts/net10.0-windows-win-x64-contained/AdaptaPELite.exe
# Salir de la app antes del siguiente script, que vuelve a abrirla:
./scripts/check-voice.ps1 -Executable ./artifacts/net10.0-windows-win-x64-contained/AdaptaPELite.exe
```

El primero comprueba la aparición de ajustes y deja la app abierta. El segundo activa Voz, exige 30 segundos de estado de escucha y la apaga al terminar. No acredita precisión de dictado: hay que hablar y comprobar resultados en el equipo real.

[VALIDACION-WINDOWS.md](docs/VALIDACION-WINDOWS.md) conserva resultados históricos por entrega, incluidos builds cruzados de los tres targets y comprobaciones portables. Esta revisión del README sólo contrastó fuentes, scripts y documentación desde macOS sin ejecutar .NET. Quedan pendientes Windows real, instalación/actualización/desinstalación, SAPI/Vosk con micrófono, UIA, siete filtros y restauración, convivencia Base/Lite, teclado/lector, DPI mixto, suspensión y rendimiento. Los resultados de macOS o Base no acreditan Lite Windows.

## Autoría, licencia y otras plataformas

© 2026 Rodrigo Alejandro Apcho Aliaga (TheAlejo160). El proyecto mantiene [Creative Commons Atribución-NoComercial-CompartirIgual 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/deed.es): consulta [LICENSE](LICENSE), [LICENSE.txt](LICENSE.txt) y [avisos de terceros](THIRD_PARTY_NOTICES.md). Los componentes, modelos y recursos de terceros conservan sus licencias. Lite reutiliza el fondo Base y conserva el logo Lite original; ICO y BMP son derivados de empaquetado.

Se utilizaron herramientas de IA generativa como asistencia en código, documentación y recursos gráficos, bajo diseño, integración y supervisión del autor. Esto no añade un proveedor de IA a la aplicación Lite.

El [sitio oficial de Adapta PE](https://www.adaptape.cc) presenta el proyecto. Para otra plataforma, consulta [macOS Lite](https://github.com/TheAlejo160/Adapta-PE-Lite-MacOS), [macOS Base](https://github.com/TheAlejo160/Adapta-PE-MacOS-Intel-Apple-Silicon-), [extensión Lite](https://github.com/TheAlejo160/Adapta-PE-Lite) o [extensión Base](https://github.com/TheAlejo160/Adapta-PE). Cada una tiene instalación, permisos y validación propios; un ZIP de la extensión no instala esta app Windows.
