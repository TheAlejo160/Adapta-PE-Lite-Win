# Memoria del código Lite

Proyecto de grafo: `Users-thealejo-Desktop-Adapta-PE-Lite-Win`. Fuente: este checkout. El índice persistente se guarda en `.codebase-memory/graph.db.zst`; se regenera después de los cambios. La fuente actual tiene prioridad ante cobertura parcial o metadatos desactualizados.

## Responsabilidades

- `App`: instancia Lite, arranque, errores WPF y eventos de sesión/energía/pantalla.
- `MainWindow`: composición exclusiva de `Voice`, `Actions`, `DesktopAccess` y `Filters`; ajustes, bandeja, filtros, parada y restauración. `FloatingWindow` conserva sólo voz/estado/elecciones; `HighlightWindow` marca selección manual.
- `Theme`: recursos congelados compartidos y estilos WPF nativos, con pack URI del ensamblado Lite.
- `Commands`, `AppNames`, `KeyNames`, `WakeState`: catálogo de sitios, parser, alias de apps, teclas y activación. El parser deriva de Base; transforma sus intenciones excluidas en `unsupported` antes de actuar sobre UIA.
- `Voice`, `WaveInput`, `LocalSpeech`, `LiveAudioStream`: SAPI/Vosk, captura WinMM, ciclo de vida de reconocimiento y contrato de audio continuo.
- `Actions`, `Applications`, `DesktopAccess`, `Native`: destino externo estable, elecciones, catálogo de apps, UIA manual y entrada Win32 verificada.
- `Filters`, `ColorMatrices`: siete matrices heredadas, Magnification API, mutex compartido Base/Lite y recuperación por edición.
- `Preferences`: sólo voz/filtro/panel/inicio voluntario; ubicación y registro propios de Lite, sin claves de proveedor.
- `installer.ps1`, `installer.nsi`: paquete autocontenido, hashes de voz, licencias, instalación/actualización/reparación y borrado acotado.

## Flujo verificado

`App.OnStartup` compone `MainWindow`, conecta recursos/eventos y restaura módulos. `Voice` entrega una orden tras `WakeState`; `MainWindow.Run` muestra intención y llama `Actions.Execute`. Las órdenes locales pasan por `HandleLocal`; las externas mantienen destino/foco y usan UIA o Win32. Las funciones Base excluidas se resuelven localmente como no disponibles. No existe captura/inferencia de cámara ni recorrido periódico de controles bajo cursor.

`StopAll` cancela acciones/elecciones, invalida reconocimiento, silencia síntesis y retira filtro propio. `Suspend` conserva intención previa para `Resume`; Detener todo borra esa intención. Salir guarda preferencias antes de liberar recursos.

## Evidencia y límites del índice

La revisión Base usó búsqueda de símbolos, trazado de composición, fragmentos y cobertura, con lectura directa de fuentes usadas. Se leyó NSIS directamente porque el grafo no lo analiza estructuralmente. El README macOS tenía metadatos cambiados y `docs/LITE.md` no estaba registrado; se verificaron las fuentes actuales.

Los artefactos, carpetas `bin`/`obj`, modelos y bibliotecas binarias pueden quedar excluidos del grafo. Se verifican mediante ensamblados, archivos y hashes, no mediante ausencia de símbolos en una búsqueda. Las comprobaciones portables inspeccionan el ensamblado publicado sin ejecutar WPF. Un índice limpio no acredita funcionamiento Windows ni exhaustividad; consultar [validación pendiente](VALIDACION-WINDOWS.md).

Índice generado el 3 de octubre de 2026: 847 nodos y 2.721 relaciones en la revisión de código. Cobertura de las 16 fuentes C# usadas sin incidencias registradas. `scripts/check-voice.ps1:3` presenta parseo parcial y se leyó directamente; los datos del modelo `word_boundary.int` también presentan parseo parcial, pero su integridad se comprueba mediante hash. El grafo no representa esos datos como lógica de aplicación. NSIS se verificó por fuente y compilación, no por relaciones del grafo.

## Actualización de voz 1.0.1

`Commands` añade frases naturales y un vocabulario finito compartido con SAPI. `AppNames` comparte alias, coincidencias de nombre y puntuación por identidad de proceso. `Applications` combina Inicio/App Paths y snapshots de ventanas con HWND/PID; los parecidos exigen elección. `Actions` reutiliza esos snapshots para enfocar/cerrar, enumera apps abiertas y comprueba destino bajo el cursor.

`Core/RecognitionSession.cs` conserva generaciones e invalida arranques que esperan liberación. `Voice.Restart` mantiene activación, espera liberar Vosk y carga SAPI con gramáticas de comandos/activación más dictado. `MainWindow.ChangeVoice` mantiene intención de escucha al cambiar ajustes. Base y Lite tienen estos motores equivalentes; Lite conserva su wrapper de comandos excluidos y composición propia.

Regeneración persistente completa para esta entrega y revisión de cobertura por fuente. NSIS se lee directamente: el índice no analiza ese lenguaje. El índice describe código, no ejecución Windows. CLI Antigravity se consultó para revisión independiente, pero no devolvió texto utilizable; las conclusiones se verificaron contra fuentes y comprobaciones locales.

Tres targets compilados desde macOS. Las comprobaciones portables verifican parser, alias, negaciones y espera/cancelación del arranque. Una compilación inicial de Lite net48 informó NU1900 por consulta de vulnerabilidades NuGet no disponible. La compilación final de los tres targets termina sin errores ni advertencias; no se acredita una auditoría nueva de paquetes. No se añaden dependencias. Precisión acústica, HWND/UIA real y cambio de motor con dispositivos siguen pendientes en Windows.

Cobertura final: las nueve fuentes C# modificadas (incluidas comprobaciones) no reportan incidencias y coinciden con el índice. `check-voice.ps1` tiene un hueco de parseo en línea 3 (`Add-Type`); esa línea se leyó directamente en ambas ediciones. `.csproj` y NSIS no se siguen como código en el grafo y se verifican directamente. `word_boundary.int` es un dato del modelo con parseo parcial, no evidencia de lógica C#; el empaquetador verifica sus hashes de manifiesto.

## Numeración unificada 1.0

`MainWindow.ProductVersion` toma el atributo informativo del ensamblado y se comparte con PiP. El proyecto define versión pública 1.0 y FileVersion 1.0.3.0. `installer.ps1` pasa ambos valores a NSIS; éste guarda DisplayVersion público y BuildVersion técnico, con fallback al registro antiguo/EXE propio. NSIS se verifica directamente porque no se analiza en el grafo. Las pruebas de lógica/metadata y compilación desde macOS no acreditan actualización Windows real.

## Publicación de fuentes — 3 de octubre de 2026

El README documenta clonación, compilación, instalación local y recorrido de cada motor. `.codebase-memory/` sigue siendo memoria regenerable del equipo y queda excluida de Git; se conserva este mapa público. Se regenera el índice completo al finalizar y se revisa cobertura de las fuentes consultadas. NSIS y archivos de proyecto se leen directamente: no se les atribuye cobertura estructural del grafo. La publicación de fuentes no acredita ejecución Windows ni una release binaria validada.
