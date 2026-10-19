# Validación de Adapta PE Lite para Windows

## Bloque de comandos: 17 de octubre de 2026

421 comprobaciones portables aprobadas: cortesía/conjugaciones, idempotencia, dictado y selección literal, negaciones, comandos históricos, puntuación tras muletillas, siglas PE en nombres, activación predeterminada y personal, expiración de ocho segundos, posiciones Unicode y reloj inválido. No se usó micrófono ni API real. Lite conserva sus exclusiones de cámara, cinética, TalkBack automático e IA.

Compilación cruzada desde macOS de net10.0-windows, net48 y net461 con SDK temporal oficial 10.0.402: cero errores y cero advertencias. Esto no acredita WPF, reconocimiento de voz, filtros ni instalación en Windows real. No se regeneró el instalador.

Fecha: 3 de octubre de 2026. Derivación de Base Windows 1.0.1; versión Lite 1.0.0. Preparación en macOS con SDK .NET 10.0.401 y NSIS 3.13. Una compilación cruzada no ejecuta WPF ni acredita funcionamiento de Windows.

## Evidencia disponible

- Compilaciones `net10.0-windows`, `net48` y `net461` aprobadas. Publicación moderna autocontenida `win-x64` producida.
- Comprobaciones portables: **324 aprobadas**; con inspección de la carpeta publicada: **329 aprobadas**. No activan micrófono ni filtros.
- Ensamblado publicado contiene `Voice` y `Filters`. No contiene `Camera`, `LocalVision`, `KineticState`, `OptionalAI` ni `DwellWindow`. Paquete sin DLL OpenCV ni modelos ONNX.
- Los **18 archivos** del manifiesto de voz coinciden con SHA-256 de Base y con el contenido publicado. Modelo español y DLL Vosk x64 incluidos.
- Logo Lite transparente `AdaptaPELite.png` copiado sin alteraciones; manifiesto de marca registra SHA-256. ICO con tamaños 16/24/32/48/64/128/256 y BMP de instalador a 24 bits derivados para empaquetado. Transparencia PNG/ICO y recursos incrustados en los tres targets verificados. BMP compuestos sobre fondos claros y banner del instalador inspeccionado visualmente.
- Dependencias importadas por las cuatro DLL de Vosk: Windows `KERNEL32.dll`/`msvcrt.dll` y bibliotecas GCC/winpthreads incluidas. No importan el runtime Visual C++ de cámara. No se empaqueta su instalador en Lite.
- Instalador NSIS compilado sin errores: **99.894.090 bytes (95,3 MiB)**. Base 1.0.1 de referencia: 156.794.024 bytes; reducción de archivo **36,3 %**. Publicación Lite: 298.416.534 bytes. No son mediciones de memoria o CPU.
- NSIS mantiene ruta/registro/accesos/instancia propios de Lite, protección de downgrade y borrado específico por archivo. Su compilación no prueba instalación/desinstalación.

Los resultados, tamaños y SHA-256 del instalador se registran en `artifacts/BUILD-REPORT.json` y `artifacts/AdaptaPELite-1.0.0-Setup-x64.sha256`. Ese informe registra artefactos y hashes; no representa una validación de ejecución.

## Comprobaciones reproducibles

En PowerShell, desde la raíz:

```powershell
dotnet build src/AdaptaPELite/AdaptaPELite.csproj -c Release
dotnet run --project tests/AdaptaPELite.Checks -c Release
./scripts/installer.ps1
dotnet run --project tests/AdaptaPELite.Checks -c Release -- artifacts/net10.0-windows-win-x64-contained
./scripts/check-startup.ps1 -Executable ./artifacts/net10.0-windows-win-x64-contained/AdaptaPELite.exe
./scripts/check-voice.ps1
```

En macOS, es posible ejecutar el ensamblado de pruebas con `dotnet <ruta>/AdaptaPELite.Checks.dll`. `EnableWindowsTargeting` y los ensamblados de referencia permiten compilar la aplicación; no permiten ejecutar WPF. Los paquetes restaurados desde caché no acreditan una consulta actual a la base de vulnerabilidades NuGet.

## Pruebas pendientes en Windows

- [ ] Instalación limpia Windows 11 x64, EXE/autocontenidos, logo en ventana/bandeja/accesos y primer inicio con todo apagado.
- [ ] Windows 10 en edición/build concretos; distinguir ejecución de soporte oficial del runtime. No acreditar Home/Pro 22H2 sólo por admitir instalación.
- [ ] Actualización desde versión menor, reparación de misma versión, bloqueo de downgrade y conservación de accesos/preferencias.
- [ ] Base y Lite instaladas juntas: carpetas, claves y desinstalación independientes; desinstalar Lite conserva Base y archivos ajenos.
- [ ] Desinstalación sin voz/filtro activos, inicio voluntario retirado sólo si apunta al EXE desinstalado. Preferencias conservadas.
- [ ] Micrófono predeterminado y seleccionado, permisos denegados, dispositivo ausente/desconectado y apagado durante carga Vosk.
- [ ] SAPI español con dictado, fallback Automático y elección SAPI explícita. Vosk x64 en equipo sin reconocimiento español instalado.
- [ ] Activación de ocho segundos, orden junto al nombre, cambios de aplicación/ventana, TTS interrumpido, elecciones de seis opciones y cancelación/caducidad.
- [ ] Dictado Unicode, contraseñas excluidas, menús/UIA, lectura manual, selección sin clic y confirmación posterior. Probar distintos navegadores/apps y restricciones UIPI.
- [ ] Los siete filtros, restauración de otro efecto, conflicto Base/Lite y otras ayudas, recuperación tras terminar abruptamente y varios monitores.
- [ ] Teclado y lector de pantalla, foco visible, alto contraste, 100/150/200 % DPI, monitor con coordenadas negativas y PiP en área útil.
- [ ] Cerrar ajustes conserva voz y bandeja. Detener todo recuerda apagado. Salir conserva preferencias. Bloqueo/suspensión apagan entrada y revalidan antes de reanudar.
- [ ] Legacy `net48` en Windows 7 SP1/8.1 y `net461` en Windows 8.0: runtime, SAPI/Vosk, filtro Windows 7 y APIs disponibles. Compilar no acredita estos escenarios.
- [ ] Medir CPU/RAM con asistencia apagada, SAPI, Vosk y cada filtro; medir arranque y tamaño instalado. No publicar porcentajes de mejora sin esas mediciones.
- [ ] Firma Authenticode, reputación SmartScreen y distribución autorizada. No pedir desactivar protecciones.

## Límites conocidos

Lite conserva los límites de voz y UIA de Base. Vosk pequeño añade memoria al activarse y no ofrece puntuación/capitalización automática equivalente a todos los motores. Los proveedores UIA pueden bloquear una llamada COM; cancelar una tarea no garantiza interrumpirla. Los filtros son simulaciones, no tratamiento médico ni reconocimiento de contenido protegido. x86 necesita Windows x86 y SAPI; ARM64 no se acredita.

No hay cámara, IA externa, lectura automática ni envío de pantalla en Lite. Las referencias macOS/web orientan contratos, pero sus pruebas no validan Windows. No se modifica ninguno de esos proyectos durante esta entrega.

## Regresión de voz y apps 1.0.1 pendiente en Windows

1. Con voz activa, cambiar Local → Automático → SAPI → Local y micrófono varias veces. Debe quedar activo sólo el último motor. Cambiar con voz apagada debe conservarla apagada.
2. Apagar/Detener todo mientras carga Vosk o libera el motor anterior. Esperar a que termine: no debe reaparecer escucha, señal ni captura. Probar suspensión durante el cambio.
3. Comparar reconocimiento hablado de «Computadora, cerrar Google», «me puedes cerrar la app Chrome porfa», clic derecho, doble clic y lectura de pantalla con SAPI y Local. Registrar reconocedor, micrófono y errores sin guardar audio/transcripciones.
4. Chrome instalado sin acceso en Inicio, Chrome portable ya abierto, varios perfiles/ventanas y otra app con documento titulado Google Chrome. Nombres mal reconocidos deben exigir elección; cerrar/reemplazar una ventana antes de elegir debe cancelar.
5. «Muestra apps abiertas», «cambia a Discord», cambiar destino antes de elegir y confirmar que Adapta no sustituye el destino. Colocar cursor fuera del destino y pedir clic: debe mostrar error sin pulsar otra ventana.
6. Dictado Unicode, texto que contiene «cerrar Chrome», negaciones y síntesis propia. Verificar pestaña anterior/siguiente para descartar conflictos con cambio de app. Lectura sigue excluyendo contraseñas.

Estos casos requieren Windows real. Las comprobaciones de generación/tareas usan una liberación controlada y no prueban el driver WinMM, SAPI ni Vosk nativo.

Comprobaciones de esta entrega: 335 sin sensores y 340 con inspección del paquete. Compilación cruzada de `net10.0-windows`, `net48` y `net461`; Una consulta inicial de vulnerabilidades NuGet no estuvo disponible; la compilación final termina sin errores ni advertencias. Instalador local x64 generado; ejecución e instalación Windows pendientes.

## Numeración pública 1.0

Validar en Windows que ventana, PiP, asistente, Configuración/Desinstalar y propiedades de versión del producto indiquen 1.0 en ambas ediciones. FileVersion técnico es 1.0.3.0. Actualizar desde Base 1.0.2 y Lite 1.0.1 (registro sin BuildVersion); reparar la misma revisión; bloquear una revisión superior. Probar EXE propio sin registro con una revisión en el cuarto componente. Conservar accesos/preferencias y comprobar desinstalación.

Verificación de la numeración unificada: 343 comprobaciones aprobadas sobre el paquete, incluida versión de producto 1.0 y revisión de archivo 1.0.3.0. Compilación cruzada sin errores ni advertencias en los tres targets; fuentes NSIS y migración de claves revisadas directamente. Instalación/actualización, WPF y reconocimiento con micrófono permanecen pendientes en Windows.
