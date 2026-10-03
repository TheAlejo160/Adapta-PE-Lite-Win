# Avisos de terceros

Adapta PE Lite para Windows deriva de Adapta-PE-Win 1.0.1 y conserva los motores de voz, accesibilidad y filtros. © 2026 Rodrigo Alejandro Apcho Aliaga — TheAlejo160. Se mantienen [LICENSE](LICENSE) y [LICENSE.txt](LICENSE.txt), CC BY-NC-SA 4.0. Las licencias de terceros conservan sus condiciones independientes.

- **.NET, WPF, Windows Forms y System.Speech:** Microsoft y colaboradores, MIT. System.Speech 10.0.12 en la edición moderna. .NET Framework no se redistribuye aquí. Los paquetes autocontenidos incluyen las licencias y avisos íntegros del runtime en `licenses/Microsoft.NETCore.App/` y `licenses/Microsoft.WindowsDesktop.App/`.
- **Vosk 0.3.45:** Alpha Cephei, Apache-2.0. Runtime Windows x64 de la [release oficial](https://github.com/alphacep/vosk-api/releases/tag/v0.3.45). API C directa, sin Python ni servidor.
- **Modelo vosk-model-small-es-0.42:** AC Technologies / Alpha Cephei, Apache-2.0 según el [catálogo oficial](https://alphacephei.com/vosk/models). Procedencia y SHA-256 conservados en `assets/models/speech/manifest.json`.
- **Bibliotecas de voz:** Kaldi, Apache-2.0; OpenBLAS, BSD; GCC libstdc++/libgcc, GPL-3.0 con GCC Runtime Library Exception; mingw-winpthreads, condiciones permisivas incluidas. Textos íntegros en `assets/speech-runtime/licenses/`, copiados al paquete.
- **NSIS 3.13:** instalador nativo bajo licencia zlib/libpng. Avisos en `assets/installer/licenses/` y `licenses/installer/` del paquete. [Licencia oficial](https://nsis.sourceforge.io/License).
- **Marca:** logo Lite original suministrado por el autor en esta petición; PNG conservado íntegro. ICO de varios tamaños y BMP del instalador derivados para empaquetado, sin redibujar el logo. Fondo del proyecto Base Windows, proveniente de la aplicación macOS del autor. `assets/brand/manifest.json` registra la imagen recibida y su SHA-256.

Lite no incluye OpenCV, OpenCvSharp ni modelos de visión OpenCV Zoo. No se descarga ningún modelo durante el uso.
