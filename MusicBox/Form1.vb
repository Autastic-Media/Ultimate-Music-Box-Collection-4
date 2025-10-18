' Created by The Autastic Media Dev Team © 2020.
' This software is provided as source-available for educational and recreational use only.
' Redistribution, modification, and use are permitted under the following conditions:
' - Non-commercial use only
' - Attribution to The Autastic Media Dev Team must be preserved
' - No warranty is provided; use at your own risk
' For full details, visit: https://github.com/Autastic-Media

Imports System.IO
Imports NAudio.Wave

Public Class Form1
    Private play_all As Boolean
    Private waveOut As WaveOutEvent
    Private audioStream As MemoryStream
    Private audioReader As WaveFileReader
    Private isLooping As Boolean = False
    Private isPaused As Boolean = False
    Private toolTip1 As ToolTip
    Private currentVolume As Single = 1.0F
    Private WithEvents CheckBox_RememberVolume As CheckBox

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If ComboBox1.SelectedIndex = -1 Then ComboBox1.SelectedIndex = 0

        toolTip1 = New ToolTip()
        toolTip1.AutoPopDelay = 5000
        toolTip1.InitialDelay = 500
        toolTip1.ReshowDelay = 200
        toolTip1.ShowAlways = True

        toolTip1.SetToolTip(btnTogglePlayback, "Pause or resume music playback")
        toolTip1.SetToolTip(Button_Clear, "Clear current selection and reset")
        toolTip1.SetToolTip(Button_Close, "Close the application")
        toolTip1.SetToolTip(Button_About, "Information about the application")
        toolTip1.SetToolTip(CheckBox1, "Enable first to loop selected music")
        toolTip1.SetToolTip(CheckBox2, "Enable this to play all music in sequence")
        toolTip1.SetToolTip(TrackBar_Volume, "Adjust playback volume")

        Dim baseLeft As Integer = TrackBar_Volume.Left
        Dim labelTop As Integer = TrackBar_Volume.Top - -28

        Label1.Left = baseLeft + 8
        Label3.Left = baseLeft + 67
        Label4.Left = baseLeft + 128
        Label5.Left = baseLeft + 190
        Label6.Left = baseLeft + 247

        Label1.Top = labelTop
        Label3.Top = labelTop
        Label4.Top = labelTop
        Label5.Top = labelTop
        Label6.Top = labelTop

        If My.Settings.LastVolume > 0 Then
            currentVolume = My.Settings.LastVolume
        Else
            currentVolume = 1.0F
        End If

        TrackBar_Volume.Value = CInt(currentVolume * 100)
    End Sub
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If Me.ComboBox1.Text = "Music Select: (Click on Loop Selected Music first, to loop selected music)" Then
            PictureBox1.Image = My.Resources.ImageBoxStillMed
            StopPlayback()
            CheckBox1.Enabled = True
        End If

        Select Case Me.ComboBox1.Text
            Case "37: Gnossiennes No.1 - Erik Satie" : PlaySelectedSong(My.Resources._37)
            Case "38: Gymnopédie No.1 - Erik Satie" : PlaySelectedSong(My.Resources._38)
            Case "39: Habanera (Carmen) - Georges Bizet" : PlaySelectedSong(My.Resources._39)
            Case "40: Hallelujah Chorus - George Frideric Handel" : PlaySelectedSong(My.Resources._40)
            Case "41: Hungarian Dance No. 5 in G minor - Johannes Brahms" : PlaySelectedSong(My.Resources._41)
            Case "42: Je Te Veux (I Want You) - Erik Satie" : PlaySelectedSong(My.Resources._42)
            Case "43: Jesu, Joy of Man's Desiring (BWV 147) - Johann Sebastian Bach" : PlaySelectedSong(My.Resources._43)
            Case "44: Judas Maccabaeus (See the Conquering Hero Comes) - Handel" : PlaySelectedSong(My.Resources._44)
            Case "45: Liebestraum No. 3 (Dream of Love) - Franz Liszt" : PlaySelectedSong(My.Resources._45)
            Case "46: Marche Militaire - Franz Schubert" : PlaySelectedSong(My.Resources._46)
            Case "47: Minuet BWV Anh. 114 - Christian Petzold (attributed to J.S. Bach)" : PlaySelectedSong(My.Resources._47)
            Case "48: Minuet in G major, WoO 10 No. 2 - Ludwig van Beethoven" : PlaySelectedSong(My.Resources._48)
        End Select
    End Sub

    Private Sub PlaySelectedSong(ByVal file As System.IO.UnmanagedMemoryStream)
        StopPlayback()

        audioStream = New MemoryStream()
        file.CopyTo(audioStream)
        audioStream.Position = 0

        audioReader = New WaveFileReader(audioStream)
        waveOut = New WaveOutEvent()
        waveOut.Init(audioReader)
        waveOut.Volume = currentVolume

        PictureBox1.Image = My.Resources.ImageBoxMoveMed
        PictureBox1.Image.Tag = "Playing"

        CheckBox1.Enabled = False
        CheckBox2.Enabled = False

        If CheckBox1.Checked Then
            CheckBox1.Text = "Press Clear to Reset"
            isLooping = True
            AddHandler waveOut.PlaybackStopped, AddressOf LoopPlayback
        Else
            isLooping = False
            Timer1.Interval = CInt(audioReader.TotalTime.TotalMilliseconds)
            Timer1.Start()
        End If

        waveOut.Play()
        isPaused = False
        btnTogglePlayback.Text = "Pause"
    End Sub

    Private Sub LoopPlayback(sender As Object, e As StoppedEventArgs)
        If isLooping Then
            audioStream.Position = 0
            audioReader = New WaveFileReader(audioStream)
            waveOut.Init(audioReader)
            waveOut.Volume = currentVolume
            waveOut.Play()
        End If
    End Sub

    Private Sub StopPlayback()
        If waveOut IsNot Nothing Then
            RemoveHandler waveOut.PlaybackStopped, AddressOf LoopPlayback
            waveOut.Stop()
            waveOut.Dispose()
            waveOut = Nothing
        End If
        If audioReader IsNot Nothing Then
            audioReader.Dispose()
            audioReader = Nothing
        End If
        If audioStream IsNot Nothing Then
            audioStream.Dispose()
            audioStream = Nothing
        End If
    End Sub

    Private Sub btnTogglePlayback_Click(sender As Object, e As EventArgs) Handles btnTogglePlayback.Click
        If waveOut IsNot Nothing Then
            If waveOut.PlaybackState = PlaybackState.Playing Then
                waveOut.Pause()
                isPaused = True
                btnTogglePlayback.Text = "Resume"
                PictureBox1.Image = My.Resources.ImageBoxStillMed
                PictureBox1.Image.Tag = "Paused"
            ElseIf isPaused Then
                waveOut.Play()
                isPaused = False
                btnTogglePlayback.Text = "Pause"
                PictureBox1.Image = My.Resources.ImageBoxMoveMed
                PictureBox1.Image.Tag = "Playing"
            End If
        End If
    End Sub

    Private Sub Button_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Close.Click
        Close()
    End Sub

    Private Sub Button_Clear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Clear.Click
        EndOfSelection()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If Me.CheckBox1.Checked = False Then
            CheckBox1.Text = "Loop Selected Music"
            Timer1.Stop()
            ComboBox1.Text = "Music Select: (Click on Loop Selected Music first, to loop selected music)"
            CheckBox1.Enabled = True
            PictureBox1.Image = My.Resources.ImageBoxStillMed
            PlayEndSound()
        End If
    End Sub

    Private Sub Button_About_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_About.Click
        Form_About.Show()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        System.Diagnostics.Process.Start("https://github.com/Autastic-Media")
    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If Me.CheckBox2.Checked Then
            CheckBox1.Enabled = False
            CheckBox2.Enabled = False
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False

            play_all = True
            Timer1.Start()
            Timer1.Interval = 1
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Timer1.Stop()

        If play_all And ComboBox1.SelectedIndex + 1 < ComboBox1.Items.Count Then
            ComboBox1.SelectedIndex += 1
        Else
            play_all = False
            EndOfSelection()
        End If
    End Sub

    Private Sub EndOfSelection()
        If waveOut IsNot Nothing Then
            RemoveHandler waveOut.PlaybackStopped, AddressOf LoopPlayback
        End If

        StopPlayback()

        isLooping = False
        play_all = False
        ComboBox1.BringToFront()
        ComboBox1.Enabled = True
        PictureBox1.Image = My.Resources.ImageBoxStillMed
        PictureBox1.Image.Tag = String.Empty
        ComboBox1.SelectedIndex = 0
        CheckBox1.Checked = False
        CheckBox2.Checked = False
        CheckBox1.Enabled = True
        CheckBox2.Enabled = True
        btnTogglePlayback.Text = "Pause"

        PlayEndSound()
    End Sub

    Private Sub PlayEndSound()
        StopPlayback()
        audioStream = New MemoryStream()
        My.Resources._End.CopyTo(audioStream)
        audioStream.Position = 0

        audioReader = New WaveFileReader(audioStream)
        waveOut = New WaveOutEvent()
        waveOut.Init(audioReader)
        waveOut.Volume = currentVolume

        PictureBox1.Image = My.Resources.ImageBoxStillMed
        PictureBox1.Image.Tag = "End"

        waveOut.Play()
        isPaused = False
    End Sub

    Private Sub TrackBar_Volume_Scroll(sender As Object, e As EventArgs) Handles TrackBar_Volume.Scroll
        Dim raw = TrackBar_Volume.Value
        Dim snapPoints() As Integer = {0, 25, 50, 75, 100}
        Dim threshold As Integer = 3

        Dim closest = snapPoints.OrderBy(Function(p) Math.Abs(p - raw)).First()
        If Math.Abs(closest - raw) <= threshold Then
            TrackBar_Volume.Value = closest
            raw = closest
        End If

        Dim newVolume As Single = raw / 100.0F
        If waveOut IsNot Nothing AndAlso waveOut.Volume <> newVolume Then
            waveOut.Volume = newVolume
        End If

        currentVolume = newVolume

        My.Settings.LastVolume = currentVolume
        My.Settings.Save()
    End Sub
End Class