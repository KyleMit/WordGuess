Namespace Common
    Public Class UserSettings : Inherits SettingsBase

#Region " My Settings"

        Public Property ShowAlphabet() As Boolean
            Get
                Return GetProperty(True)
            End Get
            Set(ByVal value As Boolean)
                SetProperty(value)
            End Set
        End Property

        Public Property ShowBestGuesses() As Boolean
            Get
                Return GetProperty(True)
            End Get
            Set(ByVal value As Boolean)
                SetProperty(value)
            End Set
        End Property

        Public Property ShowNumberPossibilites() As Boolean
            Get
                Return GetProperty(True)
            End Get
            Set(ByVal value As Boolean)
                SetProperty(value)
            End Set
        End Property

        Public Property ShowPossibilites() As Boolean
            Get
                Return GetProperty(True)
            End Get
            Set(ByVal value As Boolean)
                SetProperty(value)
            End Set
        End Property

        Public Property EnforceSpellcheck() As Boolean
            Get
                Return GetProperty(True)
            End Get
            Set(ByVal value As Boolean)
                SetProperty(value)
            End Set
        End Property

#End Region

    End Class
End Namespace