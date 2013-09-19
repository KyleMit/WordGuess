Namespace Common

    ''' <summary>
    ''' A subclass of <see cref="TextBox"/> that updates it's binding immediately on text changed.
    ''' </summary>
    ''' <remarks>http://marcominerva.wordpress.com/2013/03/14/how-to-immediately-update-the-source-of-a-textbox-in-winrt/</remarks>
    Public Class ImmediateUpdateSourceTextBox : Inherits TextBox

        Public Shared Shadows ReadOnly TextProperty As DependencyProperty = _
            DependencyProperty.Register("Text", GetType(String), GetType(ImmediateUpdateSourceTextBox), _
                                        New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnTextChanged)))

        Private Shared Sub OnTextChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim txt = TryCast(d, TextBox)
            txt.Text = DirectCast(e.NewValue, String)
        End Sub

        Public Shadows Property Text() As String
            Get
                Return DirectCast(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property

        Private Sub HandleTextChanged(s As Object, e As Object) Handles MyBase.TextChanged
            Text = MyBase.Text
        End Sub

    End Class

End Namespace