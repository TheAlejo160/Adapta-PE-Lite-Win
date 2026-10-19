# Instrucciones para desarrollar Adapta PE Lite para Windows

## Objetivo y estado

Implementar una aplicación de accesibilidad de escritorio independiente en **C#/.NET y WPF**, trasladando los comportamientos de Adapta PE y Lite de web/macOS. Priorizar Windows 10 y 11; ofrecer compatibilidad histórica separada para 7 SP1, 8.1 y 8.0 con límites explícitos.

Este repositorio deriva de Adapta-PE-Win 1.0.1 y conserva sus contratos de voz, UIA, filtros, privacidad y distribución. La edición Lite 1.0.0 incluye únicamente **asistente de voz completo y siete filtros de daltonismo**, además de bandeja, panel, preferencias y ayuda. La lectura manual solicitada por voz forma parte del asistente.

Excluir físicamente cámara, cinética, modelos ONNX, OpenCV, TalkBack automático, IA externa y almacenamiento de claves. No añadir controles inactivos para esas funciones. Los comandos de Base excluidos responden como no disponibles y nunca pasan al fallback de menús UIA. Conservar las protecciones del asistente; no recortarlo a un lanzador de sitios.

El usuario autorizó crear esta aplicación y su instalador local personalizado, con el logo Lite adjunto. No modificar Base ni los repositorios de referencia. Compilación cruzada no acredita ejecución Windows. No publicar releases sin autorización y validación real.

Lee [README.md](README.md) antes de modificar arquitectura, alcance o compatibilidad. Mantén el trabajo dentro de la petición del usuario; no cambies las cuatro fuentes de referencia sin autorización para esos proyectos.

<!-- codebase-memory-mcp:start -->
Para exploración estructural utiliza la habilidad instalada `codebase-memory`. Comprueba el proyecto activo e indexa el repositorio si no existe un índice. Usa `search_graph`, `trace_path` y `get_code_snippet` antes de búsquedas estructurales por texto. Ejecuta `check_index_coverage` para cada archivo usado como evidencia; lee directamente rangos omitidos o fuentes con índice desactualizado y declara sus límites. Usa `rg` para literales, configuración y verificación. Si las herramientas no están disponibles, indica la limitación y verifica las fuentes directamente sin afirmar una verificación por grafo.
<!-- codebase-memory-mcp:end -->

## Referencias verificadas

- macOS Base: `../TheAlejoCloud/Adapta-PE/Adapta-PE/`. Revisar `CommandParser.swift`, `VoiceManager.swift`, `VoiceInputState.swift`, `ActionManager.swift`, `ApplicationCatalog.swift`, `DesktopAccess.swift`, `TrackingManager.swift`, `KineticState.swift`, `ColorFilterManager.swift`, `OptionalAI.swift` y `Adapta_PEApp.swift` según el módulo afectado.
- macOS Lite independiente: `../TheAlejoCloud/Adapta-PE-Lite/`, con fuentes locales en `Adapta-PE-Lite/`, proyecto `Adapta-PE-Lite.xcodeproj` y `docs/LITE.md`. Incluye voz y filtros; excluye físicamente cámara, cinética, TalkBack automático e IA. No depende del proyecto Base ni de la condición `LITE`.
- Web Base: `../../PycharmProjects/WelcomeScreen/extensión/`; Lite: `../../PycharmProjects/WelcomeScreen/extensiónLite/`. Consultar `sitios.js`, `classes/VoiceAssistant.js` o `clases/VoiceAssistant.js`, filtros y sus README. Sólo Base incluye `KineticEngine`, TalkBack y cámara.
- `docs/PARIDAD.md` del proyecto macOS distingue comportamiento equivalente y límites de plataforma. Sus validaciones macOS no acreditan funcionamiento Windows. Los índices locales pueden estar desactualizados: las fuentes actuales tienen prioridad.

No importar Swift, JavaScript DOM, extensiones MV3 ni el servidor Flask para hacer funcionar la aplicación Windows. Reutilizar datos y casos de regresión mediante un port C# explícito. El prototipo Python de visión devuelve una simulación; no es un motor de inferencia reutilizable.

## Stack y política de compatibilidad

1. Interfaz WPF en C# y XAML; bibliotecas estándar y APIs públicas de Windows primero. No añadir Electron, WebView obligatorio, MAUI, WinUI 3 ni un servidor para una necesidad que resuelve WPF.
2. Edición principal `net10.0-windows` con .NET 10 LTS. Validar Windows 11 y Windows 10 prioritariamente; Windows 10 Home/Pro 22H2 es objetivo de pruebas de ejecución, no una afirmación de soporte oficial de .NET 10. Revisar la matriz vigente enlazada en README antes de cada lanzamiento.
3. Legacy `net48` para Windows 7 SP1 y 8.1, y como vía de compatibilidad Windows 10 cuando corresponda. Nunca sustituir por 4.8.1 si se mantiene 7/8.1.
4. Windows 8.0 requiere una compilación distinta `net461`, con runtime sin soporte. Implementarla al final, como compatibilidad histórica documentada; no omitirla ni afirmar que `net48` funciona allí.
5. Reutilizar fuentes compartidas cuando compilen en cada target. No bajar toda la aplicación a `net461`, copiar motores completos por runtime ni imponer .NET Standard 2.0 al target 4.6.1. Usar bloques de compilación pequeños sólo en límites realmente distintos.
6. Comprobar capacidades al ejecutar y excluir llamadas a APIs ausentes. Guardar el motivo de indisponibilidad para mostrarlo; una función no disponible no debe aparentar éxito.
7. Inicialmente x64. La magnificación necesita arquitectura nativa: x86 sólo en Windows x86, nunca un proceso x86 bajo WOW64. ARM64 exige pruebas de toda dependencia nativa. No utilizar AnyCPU sin revisar este requisito.

Versiones de runtime, paquetes, mínimos de sistema y contratos nativos deben verificarse en documentación oficial actual. Fijar versiones estables compatibles cuando se incorporen dependencias; no escoger paquetes preview por defecto.

## Organización mínima

Estructura actual:

- `src/AdaptaPELite/App.cs`, `MainWindow.cs` y `Theme.cs`: WPF, bandeja, panel sin captura de foco, suspensión y composición de voz/filtros.
- `Core`: parser, sitios, alias, activación, contrato de audio continuo y matrices. No incluye física cinética.
- `Windows`: voz SAPI/Vosk/WinMM, UIA manual, entrada Win32, apps, filtros y preferencias.
- `assets/models/speech`: sólo reconocimiento local español, bajo demanda.
- `tests/AdaptaPELite.Checks`: comprobaciones portables y exclusiones del paquete compilado.
- `scripts`: publicación y NSIS, además de comprobaciones de arranque/voz en Windows.

Añadir archivos sólo cuando una funcionalidad los necesite. MVVM sencillo cuando ayude a enlazar estado y UI; no añadir contenedor DI, bus de eventos genérico, repositorio de datos, base SQL ni jerarquías de proveedores anticipadas. Antes de escribir una función, localizar el patrón equivalente y sus llamadores; corregir el fallo en el punto común responsable.

## Contratos que deben conservarse

### Voz y comandos

- Commands.Prepare normaliza cortesía y conjugaciones sin alterar el contenido del dictado, selección literal o consultas API. Conservar el catálogo existente y su gramática finita. Las partículas ambiguas finales pe/pues/ya pe/ya pues requieren coma para no truncar nombres o consultas. Añadir regresiones de compatibilidad al ampliar verbos; no resolver cadenas del corpus con excepciones aisladas.

- Una captura/reconocedor por sesión activa. Inicio, parada y recuperación serializados; cancelación o generación descarta callbacks anteriores. Apagar durante el arranque nunca debe reactivar un módulo.
- Palabra predeterminada «Computadora», personalizable; señal de disponibilidad y ventana de ocho segundos entre activación y orden. Mantenerla entre reinicios de reconocimiento y cambios de ventana.
- Enumerar con `InstalledRecognizers()` los motores españoles realmente disponibles. Preferir el idioma del usuario y variantes españolas instaladas; no asumir `es-PE` disponible. Verificar gramática de órdenes y dictado. `System.Speech` usa referencia Framework en Legacy y paquete Microsoft compatible en moderno.
- El flujo WinMM hereda `LiveAudioStream`: longitud desconocida (-1), posición acumulada y consulta de posición sin fingir rebobinado. SAPI consulta estos datos aunque el stream no permita búsqueda.
- Automático prefiere SAPI español y permite fallback Vosk local x64 incluido. Cargar/liberar modelo bajo demanda, sin red ni transcripciones persistidas. No prometer consumo equivalente a SAPI: el modelo pequeño añade memoria sólo cuando está activo.
- WinMM usa `CALLBACK_EVENT` y un hilo acotado que recoge/reencola buffers en orden; nunca llamar funciones de audio desde `waveInProc`. Al terminar inesperadamente SAPI, Automático cambia una vez a Local; SAPI explícito informa el error real. No reintroducir reintentos sobre un reconocedor terminado.
- Ausencia de micrófono/reconocedor/voz TTS debe producir diagnóstico recuperable. No cambiar silenciosamente a un proveedor de red ni declarar que un paquete de idioma garantiza dictado.
- Procesar cada frase una sola vez. Los provisionales pueden corregir texto completo; no contar palabras para recortar finales. El dictado conserva Unicode, signos y mayúsculas; normalizar sólo intenciones y nombres.
- Mantener alias y rutas del catálogo web. Construir consultas con codificación URL; aperturas web sólo HTTP/HTTPS. Catálogo de apps separado y seguro: una orden de voz no se convierte en PowerShell, `cmd.exe`, script o argumentos arbitrarios.
- Coincidencia exacta antes de parcial; ambigüedad ofrece opciones numeradas. Elegir sólo una coincidencia válida, cancelar al cambiar la app de destino y permitir «cancelar». No guardar selección ni transcripción.
- `Core/AppNames.cs` comparte alias/canonización para abrir y cerrar. Cierre por nombre enumera ventanas visibles, prioriza proceso y no utiliza un título web para cerrar otra app conocida. Coincidencias aproximadas siempre exigen elección. Revalidar HWND/PID antes de `WM_CLOSE`; nunca forzar terminación de procesos.
- Evitar órdenes derivadas de la síntesis propia. Durante lectura aceptar interrupciones explícitas y conservar una vía accesible de parada. Errores recuperables usan espera progresiva; permisos denegados no generan reintento infinito.
- Traducir atajos a Windows por acción: guardar `Ctrl+S`, pestaña `Ctrl+T`, escritorio `Win+D`; no copiar códigos de tecla macOS.

### Acciones sobre escritorio y accesibilidad

- Preferir patrones UIA apropiados (`Invoke`, `Value`, `Text`, selección/foco) a coordenadas o pulsaciones ciegas. Comprobar soporte del patrón y estado visible/habilitado/editable.
- Excluir elementos protegidos (`IsPassword`) de lectura, selección de texto y dictado. No inspeccionar contenido secreto para resolver nombres.
- Fijar la aplicación externa de destino y revalidar proceso, ventana, elemento y foco inmediatamente antes de actuar. La UI propia no debe reemplazar el destino al mostrar opciones o respuestas.
- Si un elemento desaparece, no sustituirlo por otro coincidente. Recorridos UIA con presupuesto de tiempo/nodos y cancelación; fuera del hilo WPF cuando puedan bloquear. Proveedores externos pueden colgar: no prometer que cancelar una tarea interrumpe una llamada COM bloqueada.
- `SendInput` como alternativa limitada: comprobar número de eventos enviados, liberar modificadores y respetar UIPI. No automatizar UAC, escritorio seguro ni elevar toda la app para ocultar restricciones.
- `uiAccess` sólo con una necesidad comprobada, firma y ubicación de instalación válidas. No alterar políticas de seguridad del equipo como paso de instalación.
- Navegador: atajos conocidos/UIA y URLs seguras. No prometer equivalencia de DOM, `chrome.tabs`, selección de pestañas o búsqueda contextual sin una prueba del navegador concreto.
- Lectura manual en ambas ediciones; TalkBack automático sólo Base, con debounce de 300 ms, resaltado y consultas limitadas. No afirmar sustitución de Narrador/NVDA.

### Filtros y recursos globales

- Conservar las siete matrices de `FiltrosDaltonismo` y `ColorFilterManager`. «Sin filtro» restaura el estado anterior apropiado; no sobrescribir sin comprobar filtros de otras ayudas.
- Prototipo inicial con Magnification API: `MagInitialize`, comprobación de efecto global desde Windows 8 y `MagUninitialize`. Validar formato 5×5, transposición y alfa mediante colores de referencia.
- Respetar arquitectura nativa y errores de API. Windows 7 no ofrece `MagSetFullscreenColorEffect`: evaluar control local de magnificación y declarar falta de filtro global hasta probar una alternativa.
- Si el prototipo falla, evaluar captura/composición moderna con exclusión de overlays, límites de memoria y FPS. No construir ese pipeline antes de conocer la necesidad. No prometer capturar contenido protegido.
- El efecto global puede competir con otra aplicación. Serializar Base/Lite, comprobar estado al restaurar y evitar que una instancia deshaga el filtro de otra. Liberar recursos en parada/salida; documentar recuperación tras terminación abrupta antes de distribuir.
- Las matrices son simulaciones visuales, no una corrección médica del daltonismo.

### Ciclo de vida, UI y privacidad

- Reutilizar el fondo Base y el logo Lite adjunto en `assets/brand`. Usar el ICO Lite en ejecutable, ventanas, bandeja e instalador. Conservar el PNG original íntegro y su procedencia; no sustituirlo por el logo Base ni por iconos genéricos.
- UI en español, nombres accesibles, navegación de teclado, foco visible y contraste suficiente. Estado de error comprensible y acceso permanente a «Detener todo». No depender sólo de color, hover o voz.
- Primer inicio apagado; restauraciones posteriores sólo con capacidades/autorizaciones disponibles. Inicio con Windows voluntario, por usuario; no servicio de sistema obligatorio.
- Cerrar ajustes mantiene asistencia, bandeja y panel sin tomar foco. El panel se ancla al área útil del monitor/esquina elegidos, se adapta al DPI y ofrece elecciones accesibles sin tapar la barra de tareas.
- `StopAll` debe ser idempotente: invalidar arranques pendientes, cancelar acciones y selección, apagar voz/TTS y filtros, liberar recursos y recordar módulos apagados. Salir libera recursos conservando preferencias; no confundir ambos comportamientos.
- Bloqueo/cambio de sesión/suspensión detienen entrada y sensores. Reanudar revalida dispositivo, permisos, monitor y foco; no se reanuda fuera del escritorio de usuario. No impedir el reposo del equipo para sostener la escucha.
- Configuración separada por edición en `%LOCALAPPDATA%`; escritura con reemplazo seguro y recuperación ante archivo dañado. No guardar imágenes, audio, postura corporal ni transcripciones. Logs sólo de estado/errores sin contenido privado.
- Windows y macOS tienen modelos de permisos distintos. Explicar el uso de micrófono y consultar fallos/políticas reales; no diseñar un supuesto diálogo de Accesibilidad macOS en Windows.

## Pruebas, distribución y definición de terminado

Cada cambio de lógica no trivial deja una comprobación ejecutable mínima que falle ante la regresión; ampliar las existentes antes de crear infraestructura nueva. Lógica pura sin micrófono/UIA real en las pruebas rápidas; integración física por separado. Pruebas nunca usan claves ni proveedores reales por defecto.

Para cambios implementados, compilar los targets afectados en Windows y ejecutar comprobaciones. Priorizar parser Unicode, deduplicación, apagado durante inicio, pérdida/retorno, matrices y exclusiones Lite. Probar UI por teclado y lector, bandeja/panel, bloqueo y suspensión, DPI mixto, permisos y dispositivos reales según el módulo.

Si se trabaja desde macOS, se pueden revisar fuentes y lógica portable, pero declarar como pendientes WPF/Win32, voz, filtros e instaladores. No presentar pruebas macOS o una compilación cruzada como validación Windows.

Distribución: paquetes por edición, runtime y arquitectura. Verificar dependencias nativas, firma Authenticode, prerrequisitos, inicio voluntario y desinstalación sin residuos activos. MSIX no es la única vía si se pretende cubrir Windows 7/8; evaluar instalador MSI/EXE para Legacy. No pedir desactivar antivirus, SmartScreen ni UAC. Publicar sólo cuando el usuario haya autorizado la distribución.

Instalador actual: NSIS 3.13, sólo Windows 10/11 x64, por usuario en `%LOCALAPPDATA%\Programs\AdaptaPELite`, .NET autocontenido y voz Vosk x64 incluida, sin prerrequisito Visual C++ de cámara. `scripts/installer.ps1` genera lista de borrado específica del paquete. Conservar archivos ajenos, preferencias y runtimes compartidos; no usar `RMDir /r $INSTDIR`. No elevar la app al finalizar ni habilitar inicio con Windows durante instalación. Cerrar la app antes de actualizar/desinstalar.

Actualizar README al añadir comandos, funciones, requisitos o resultados de validación. Actualizar AGENTS al cambiar responsabilidades o contratos. No enlazar archivos inexistentes como instrucciones disponibles ni inventar URLs de releases. Conservar atribuciones y licencias de fuentes/modelos; añadir los avisos correspondientes antes de empaquetar.

Una entrega está terminada cuando el comportamiento solicitado funciona en los targets probados, las comprobaciones pertinentes pasan, Lite conserva su alcance y la documentación separa lo validado de las limitaciones. Informar de forma breve qué cambió, qué se verificó y qué queda sin verificar; no afirmar compatibilidad universal.

### Identidad y entrega Lite

- Ensamblado/EXE `AdaptaPELite`, instancia `Local\AdaptaPE.Lite.Instance`, preferencias `%LOCALAPPDATA%\AdaptaPELite`, inicio voluntario HKCU `AdaptaPELite`, instalación `%LOCALAPPDATA%\Programs\AdaptaPELite`, desinstalador HKCU `AdaptaPE.Lite`. No modificar claves/archivos Base.
- Conservar el mutex compartido `Local\AdaptaPE.ColorFilter`; Base y Lite no aplican filtros simultáneos. Recuperación local propia por edición.
- PiP sólo muestra Voz, transcripción, respuesta, elecciones, Ajustes y Detener todo. Sin timers de cámara, preview, lectura bajo cursor ni animaciones continuas. Imágenes congeladas en caché.
- NSIS conserva instalación/actualización/reparación, bloqueo de downgrade y accesos elegibles. BMP de 24 bits con logo Lite. Comprobar dependencias nativas de voz y hashes del manifiesto de voz antes de empaquetar.
- `dotnet run --project tests/AdaptaPELite.Checks -c Release -- <carpeta-publicada>` comprueba exclusiones en los tipos del ensamblado y archivos del paquete. No sustituye probar voz/filtros/instalación en Windows.
- Regenerar índice persistente `Users-thealejo-Desktop-Adapta-PE-Lite-Win` al terminar. Revisar cobertura de fuentes; leer NSIS directamente. `docs/MEMORIA-CODIGO.md` conserva límites del índice.

### Voz y comandos 1.0.1

- `Core/RecognitionSession.cs` invalida arranques anteriores y espera liberación del motor. Cambiar motor/micrófono/palabra conserva intención de escucha; Detener todo cancela la espera. Mantener regresión portable de cambios rápidos y parada durante liberación.
- `Commands.NaturalActions`, `NaturalShortcuts` y `SpeechPhrases` conservan frases finitas compartidas con SAPI. No reemplazar dictado libre ni hacer coincidencia aproximada de verbos de acción.
- `Applications.OpenWindows`/`FindRunning` conservan HWND/PID, nombre de proceso y título. App Paths complementa Inicio. Alias de Google identifican Chrome; sitio Google explícito conserva navegación web. Parecidos requieren elección y deben mantener destino externo.

### Versión pública y revisión técnica

- Base y Lite muestran versión pública `1.0`, definida en `Version` del proyecto y leída del atributo informativo por MainWindow/PiP. No volver a mantener números literales independientes en la UI.
- `FileVersion` es revisión técnica de cuatro componentes, actualmente `1.0.3.0`. NSIS recibe `VERSION` público y `FILE_VERSION` técnico; registra DisplayVersion y BuildVersion por separado. Comparar revisión técnica y conservar fallback para instalaciones anteriores y EXE propio, incluidos los cuatro componentes. No retirar protección de downgrade para renombrar una entrega.

## Mascotas y paridad de escritorio (2026-10-11)

`Core/Mascot.cs` contiene secuencias/parser; `Windows/Mascot.cs` mantiene únicamente el atlas elegido y un temporizador por duración original. Los recursos locales están en `assets/mascotas/`; conservarlos en build/publish. Mantener comandos completos para no interceptar búsquedas o dictado, callbacks TTS actuales, arrastre/teclado, DPI, monitor/posición relativa y suspensión. Lite incluye mascota, voz y filtros; no copiar motores cinéticos o IA. Las siete matrices y los límites de pantalla completa siguen documentados en README. La comprobación en macOS compila las tres variantes; no acredita ejecución Windows.

## Panel y actividad de mascota (2026-10-11)

Reutilizar `FloatingWindow`, `Preferences` y `Mascot`; no añadir otro motor. PiP tiene arrastre nativo, posición normalizada por monitor, esquinas y pie fijo de emergencia. `MascotLinked` une los movimientos y reserva 164 puntos adaptados al DPI; separar recupera coordenadas propias de la mascota. Menú y voz pasan por el mismo parser. No escribir preferencias por cada frame del arrastre; guardar al terminar. Animar laptop/teléfono una vez y lectura/habla durante TTS real. Un temporizador de inactividad de 90 segundos duerme y detiene animación; distinguir sueño manual y automático y mantener TTS activo sin descanso automático. Lógica portable no acredita WPF, DPI mixto, sensores o juegos en Windows real.
