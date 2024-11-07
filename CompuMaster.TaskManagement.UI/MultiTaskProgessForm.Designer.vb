<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MultiTaskProgessForm
    Inherits BaseForm

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MultiTaskProgessForm))
        FlowLayoutPanelForProgressingTasks = New FlowLayoutPanel()
        ProgressBar1 = New ProgressingTaskControl()
        ProgressBar2 = New ProgressingTaskControl()
        ProgressBar3 = New ProgressingTaskControl()
        ButtonStart = New Button()
        ButtonCancel = New Button()
        TimerRefreshAllControls = New Timer(components)
        ToolTipProgressErrors = New ToolTip(components)
        ButtonClose = New Button()
        ButtonsPanel = New Panel()
        ButtonCopyResultsToClipboard = New Button()
        ToolTipProgressInfo = New ToolTip(components)
        FlowLayoutPanelForProgressingTasks.SuspendLayout()
        ButtonsPanel.SuspendLayout()
        SuspendLayout()
        ' 
        ' FlowLayoutPanelForProgressingTasks
        ' 
        FlowLayoutPanelForProgressingTasks.AutoScroll = True
        FlowLayoutPanelForProgressingTasks.Controls.Add(ProgressBar1)
        FlowLayoutPanelForProgressingTasks.Controls.Add(ProgressBar2)
        FlowLayoutPanelForProgressingTasks.Controls.Add(ProgressBar3)
        FlowLayoutPanelForProgressingTasks.Dock = DockStyle.Fill
        FlowLayoutPanelForProgressingTasks.Location = New Point(0, 0)
        FlowLayoutPanelForProgressingTasks.Name = "FlowLayoutPanelForProgressingTasks"
        FlowLayoutPanelForProgressingTasks.Size = New Size(800, 498)
        FlowLayoutPanelForProgressingTasks.TabIndex = 3
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        ProgressBar1.Location = New Point(3, 3)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(772, 150)
        ProgressBar1.TabIndex = 11
        ' 
        ' ProgressBar2
        ' 
        ProgressBar2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        ProgressBar2.Location = New Point(3, 159)
        ProgressBar2.Name = "ProgressBar2"
        ProgressBar2.Size = New Size(772, 150)
        ProgressBar2.TabIndex = 12
        ' 
        ' ProgressBar3
        ' 
        ProgressBar3.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        ProgressBar3.Location = New Point(3, 315)
        ProgressBar3.Name = "ProgressBar3"
        ProgressBar3.Size = New Size(772, 150)
        ProgressBar3.TabIndex = 13
        ' 
        ' ButtonStart
        ' 
        ButtonStart.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ButtonStart.Location = New Point(713, 11)
        ButtonStart.Name = "ButtonStart"
        ButtonStart.Size = New Size(75, 23)
        ButtonStart.TabIndex = 40
        ButtonStart.Text = "&Start"
        ButtonStart.UseVisualStyleBackColor = True
        ' 
        ' ButtonCancel
        ' 
        ButtonCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ButtonCancel.Location = New Point(713, 11)
        ButtonCancel.Name = "ButtonCancel"
        ButtonCancel.Size = New Size(75, 23)
        ButtonCancel.TabIndex = 50
        ButtonCancel.Text = "&Abbrechen"
        ButtonCancel.UseVisualStyleBackColor = True
        ' 
        ' TimerRefreshAllControls
        ' 
        TimerRefreshAllControls.Interval = 800
        ' 
        ' ToolTipProgressErrors
        ' 
        ToolTipProgressErrors.AutoPopDelay = 60000
        ToolTipProgressErrors.InitialDelay = 1000
        ToolTipProgressErrors.ReshowDelay = 500
        ToolTipProgressErrors.ShowAlways = True
        ToolTipProgressErrors.ToolTipTitle = "Fehler bei der Ausführung"
        ' 
        ' ButtonClose
        ' 
        ButtonClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        ButtonClose.Location = New Point(713, 11)
        ButtonClose.Name = "ButtonClose"
        ButtonClose.Size = New Size(75, 23)
        ButtonClose.TabIndex = 51
        ButtonClose.Text = "&Schließen"
        ButtonClose.UseVisualStyleBackColor = True
        ' 
        ' ButtonsPanel
        ' 
        ButtonsPanel.Controls.Add(ButtonCopyResultsToClipboard)
        ButtonsPanel.Controls.Add(ButtonClose)
        ButtonsPanel.Controls.Add(ButtonStart)
        ButtonsPanel.Controls.Add(ButtonCancel)
        ButtonsPanel.Dock = DockStyle.Bottom
        ButtonsPanel.Location = New Point(0, 498)
        ButtonsPanel.Name = "ButtonsPanel"
        ButtonsPanel.Size = New Size(800, 46)
        ButtonsPanel.TabIndex = 52
        ' 
        ' ButtonCopyResultsToClipboard
        ' 
        ButtonCopyResultsToClipboard.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        ButtonCopyResultsToClipboard.Location = New Point(12, 11)
        ButtonCopyResultsToClipboard.Name = "ButtonCopyResultsToClipboard"
        ButtonCopyResultsToClipboard.Size = New Size(238, 23)
        ButtonCopyResultsToClipboard.TabIndex = 52
        ButtonCopyResultsToClipboard.Text = "In die &Zwischenablage kopieren"
        ButtonCopyResultsToClipboard.UseVisualStyleBackColor = True
        ButtonCopyResultsToClipboard.Visible = False
        ' 
        ' ToolTipProgressInfo
        ' 
        ToolTipProgressInfo.AutoPopDelay = 60000
        ToolTipProgressInfo.InitialDelay = 1000
        ToolTipProgressInfo.ReshowDelay = 500
        ToolTipProgressInfo.ShowAlways = True
        ToolTipProgressInfo.ToolTipTitle = "Information"
        ' 
        ' MultiTaskProgessForm
        ' 
        AcceptButton = ButtonStart
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = ButtonCancel
        ClientSize = New Size(800, 544)
        Controls.Add(FlowLayoutPanelForProgressingTasks)
        Controls.Add(ButtonsPanel)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "MultiTaskProgessForm"
        Text = "Aufgaben in Bearbeitung"
        FlowLayoutPanelForProgressingTasks.ResumeLayout(False)
        ButtonsPanel.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents FlowLayoutPanelForProgressingTasks As FlowLayoutPanel
    Friend WithEvents ProgressBar1 As ProgressingTaskControl
    Friend WithEvents ProgressBar2 As ProgressingTaskControl
    Friend WithEvents ProgressBar3 As ProgressingTaskControl
    Friend WithEvents ButtonStart As Button
    Friend WithEvents ButtonCancel As Button
    Friend WithEvents TimerRefreshAllControls As Timer
    Friend WithEvents ToolTipProgressErrors As ToolTip
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonsPanel As Panel
    Friend WithEvents ToolTipProgressInfo As ToolTip
    Friend WithEvents ButtonCopyResultsToClipboard As Button
End Class
