# Aufgabe 2 - Language Model

| Gruppenmitglieder |
|-|
| Robin Augenstein |
| Kai Petelski |
| Jannick Gutekunst |

## TOC

- [Aufgabe 2 - Language Model](#aufgabe-2---language-model)
  - [TOC](#toc)
  - [Bauen der Programme](#bauen-der-programme)
  - [Starten der Programme](#starten-der-programme)

## Bauen der Programme

Die Anwendung kann mithilfe von Docker gebaut werden. Hierfür in das Hauptverzeichnis des Repositories wechseln und `docker build . -t language_model` ausführen. Hierdurch werden die Programme gebaut und direkt ausführbar in einem Docker-Image bereitgestellt.

Im Image wird kein `ENTRYPOINT` und kein `CMD` gesetzt, damit der Anwender flexibel auswählen kann, welches Programm er mit welchen Dateien bzw. Parametern verwenden möchte.

## Starten der Programme

Auf Basis des zuvor erstellten Docker-Images können die Anwendungungen innerhalb eines Docker-Containers direkt gestartet werden. Nachdem dieses wie beschrieben erstellt wurde, können die Programme mithilfe eines Befehls nach dem Muster `docker run language_model dotnet <Programmname>.dll <Parameter>` gestartet werden. Hierbei müssen die Platzhalter `<Programmname>` und `<Parameter>` durch die gewünschten Werte ersetzt werden.

- Folgende Programme können gestartet werden:

  | Programmname | Beispiel für den Befehl |
  |-|-|
  | `Learn` | `docker run language_model dotnet Learn.dll` |
  | `Perplexity` | `docker run language_model dotnet Perplexity.dll` |

- Die Programme akzeptieren folgende Aufrufparameter:
  - `Learn`

    | Parameter | Bedeutung |
    |-|-|
    | `-i`, `--inputFilePath` | Required. Path to file with input text to learn. |
    | `-o`, `--outputFilePath` | Required. Filepath that the learned language model will be saved to using ARPA representation. If the file already exists, it will be overwritten! |
    | `-s`, `--smoothing` | Required. Type of smoothing to apply. Supported values: "Regular" and "KneserNey" |
    | `-v`, `--verbose` | Enable verbose logging. |
    | `--help` | Display help screen. |

  - `Perplexity`

    | Parameter | Bedeutung |
    |-|-|
    | `-m`, `--model` | Required. Path to a file containing the language model to use for perplexity calculation. Must be in ARPA format. |
    | `-i`, `--text` | Required. The text to calculate the perplexity of. |
    | `-v`, `--verbose` | Enable verbose logging. |
    | `--help` | Display help screen. |

- Beispiel
  - Programm `Learn`
  - Eingabedatei `/data/1_sample_text.txt`
  - Ausgabedatei `/data/out_regular.txt`
  - Smoothing: Regular
  - Datenaustausch zwischen Host und Container über Volume, Pfad zum Volume `./data`
  - Als Befehl ergibt sich hieraus:  
    `docker run -v $(pwd)/data:/data language_model dotnet learn.dll -i /data/1_sample_text.txt -o /data/out_regular.arpa -s Regular`
