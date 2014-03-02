GUIConsole
==========

UnityのコンソールのログをGameView内に表示するGUIウィンドウです。

## 使い方
1. Scene内のGameObjectにGUIConsoleクラスをアタッチする。
1. ランタイム時に、GUIConsoleインスタンスのShow関数をコールするとGameViewにコンソールウィンドウが表示される。

## できること
* GameViewでログ・エラーの表示(ログ・エラーの発生箇所（ファイル名や関数名も表示可能)
* ログのエクスポート

## クラス
### GUIConsole.cs
ログの取得し、GameViewにコンソールウィンドウを表示するクラス。
* ConsoleSize(Vector2) : コンソールウィンドウの縦横のサイズ(px)。画面サイズより大きい値が設定された場合は最適化される。
* MaxLogCount(int) : ログの最大保持数。

### GUIConsoleButton.cs
GUIConsoleの表示/非表示を切り替えるGUIボタンを表示するクラス。
（このクラスは使用しなくてもOK。)
* ButtonRect(Rect) : コンソールウィンドウの表示/非表示の切り替えをするボタンの表示位置・サイズの指定

## ログのエクスポートについて
iOS / Android端末ではコンソールウィンドウのExport Logボタンを押すとメール画面が起動して、本文にログの内容が出力される。  
Unityエディターではクリップボードにログの内容がコピーされる。

## 備考
* Unityエディター上とiOS/Android端末上ではログの出力内容に差異がある。  
(iOS/Android端末上ではログ・エラーの行数までは特定できない)











