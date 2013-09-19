Imports Windows.UI.ApplicationSettings
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives

Namespace Flyouts

    Partial Public Class SettingsHelper

        Private ReadOnly popup As New Popup()
        Private ReadOnly commands As New List(Of ICommandInfo)

        Public Sub New()
            AddHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf CommandsRequestedHandler
        End Sub

        Private Sub CommandsRequestedHandler(ByVal sender As SettingsPane, ByVal args As SettingsPaneCommandsRequestedEventArgs)
            For Each item As ICommandInfo In commands
                Dim command = New SettingsCommand(item.Key, item.Text, Sub(c) Show(item.Instance(), item.Width))
                args.Request.ApplicationCommands.Add(command)
            Next

        End Sub

        Public Sub AddCommand(Of T As {UserControl, New})(text As String, Optional key As String = Nothing, Optional width As PanelWidths = PanelWidths.Small)
            Dim cmd As New CommandInfo(Of T)() With {
                    .Key = If(key, text),
                    .Text = text,
                    .Width = CInt(width)
                    }

            If commands.FirstOrDefault(Function(c) c.Key.Equals(cmd.Key)) Is Nothing Then
                commands.Add(cmd)
            End If

        End Sub

        Private Function Show(control As UserControl, width As Double) As Popup
            If control Is Nothing Then Throw New Exception("Control is not defined")
            If Double.IsNaN(width) Then Throw New Exception("Width is not defined")

            ' layout
            popup.Width = width
            popup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right
            popup.Height = Window.Current.Bounds.Height
            popup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - popup.Width)

            ' make content fit
            popup.Child = control
            control.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
            control.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
            control.Height = popup.Height
            control.Width = popup.Width

            ' add pretty animation(s)
            popup.ChildTransitions = New Windows.UI.Xaml.Media.Animation.TransitionCollection() From { _
                New Windows.UI.Xaml.Media.Animation.EntranceThemeTransition() With {
                    .FromHorizontalOffset = 20.0,
                    .FromVerticalOffset = 0.0
                    }
                }

            ' setup
            popup.IsLightDismissEnabled = True
            popup.IsOpen = True

            ' handle when it closes
            RemoveHandler popup.Closed, AddressOf popup_Closed
            AddHandler popup.Closed, AddressOf popup_Closed

            ' handle making it close
            RemoveHandler Window.Current.Activated, AddressOf Current_Activated
            AddHandler Window.Current.Activated, AddressOf Current_Activated

            ' return
            Return popup
        End Function

        Private Sub Current_Activated(sender As Object, e As Windows.UI.Core.WindowActivatedEventArgs)
            If popup Is Nothing Then Return
            If e.WindowActivationState = Windows.UI.Core.CoreWindowActivationState.Deactivated Then
                popup.IsOpen = False
            End If
        End Sub

        Private Sub popup_Closed(sender As Object, e As Object)
            RemoveHandler Window.Current.Activated, AddressOf Current_Activated
            If popup Is Nothing Then Return
            popup.IsOpen = False
        End Sub

        Private Interface ICommandInfo
            Property Key() As String
            Property Text() As String
            Property Width() As Double
            Function Instance() As UserControl
        End Interface

        Private Class CommandInfo(Of T As {UserControl, New}) : Implements ICommandInfo

            Public Property Key() As String Implements ICommandInfo.Key
            Public Property Text() As String Implements ICommandInfo.Text
            Public Property Width() As Double Implements ICommandInfo.Width

            Public Function Instance() As UserControl Implements ICommandInfo.Instance
                Return New T()
            End Function

        End Class

        Public Enum PanelWidths As Integer
            Small = 346
            Large = 646
        End Enum

    End Class

End Namespace