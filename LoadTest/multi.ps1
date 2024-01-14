# スクリプトの内容
$executablePath = "$PWD\bin\Debug\net8.0\LoadTest.exe" # 実行ファイルのパス
$tabCount = 2 # 実行するタブ数

# Windows Terminalで複数のタブを開くコマンド
# controller
wt -w 0 nt --title "Tab $i" powershell -Command "& '$executablePath' controller"

# Windows Terminalで複数のタブを開くコマンド
# worker
for ($i = 1; $i -le $tabCount; $i++) {
    wt -w 0 nt --title "Tab $i" powershell -Command "& '$executablePath' worker"
}