
Namespace Flyouts

    Public NotInheritable Class EnterSecretWord : Inherits UserControl

        Public Property InputName As String

        Private Sub SaveName(sender As Object, e As RoutedEventArgs)
            InputName = TextInput.Text
        End Sub

    End Class

End Namespace