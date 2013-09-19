Imports Windows.Storage.Streams
Imports Windows.Storage

Namespace Model
    Public Class AppCommon

        Private Shared _wordList As List(Of String)
        Public Shared Property WordList() As List(Of String)
            Get
                Return _wordList
            End Get
            Set(ByVal value As List(Of String))
                _wordList = value
            End Set
        End Property

    End Class
End Namespace