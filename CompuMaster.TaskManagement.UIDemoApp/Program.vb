Module Program
    <STAThread()>
    Sub Main()
        ' Setzen des DPI-Modus nur, wenn auf net5.0 oder höher kompiliert wird.
#If NET5_0_OR_GREATER Then
        Application.SetHighDpiMode(HighDpiMode.DpiUnaware)
#End If

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New MainForm())
    End Sub
End Module