Imports WordSearch.Utilities
Imports WordSearch.Common

Namespace Model

    Public Class GuessedWords : Inherits BindableBase

#Region " Private Variables             "

        Private _secretWord As String = ""
        Private _lastBeforeSecret As String = ""
        Private _firstAfterSecret As String = ""
        Private _wordBetween As String = GetBestGuesses()
        Private _lastGuessWasCorrect As Boolean
        Private _guessedWords As New ObservableRangeCollection(Of String)
        Private _results As New ObservableRangeCollection(Of String)
        Private _remainingWords As New ObservableRangeCollection(Of String)
        Private _numberCharactersResolved As Integer

#End Region

#Region " Public Properties             "

        Public Property SecretWord As String
            Get
                Return _secretWord
            End Get
            Set(ByVal value As String)
                SetProperty(_secretWord, value)
            End Set
        End Property
        Public Property LastBeforeSecret As String
            Get
                Return _lastBeforeSecret
            End Get
            Set(ByVal value As String)
                SetProperty(_lastBeforeSecret, value)
            End Set
        End Property
        Public Property FirstAfterSecret As String
            Get
                Return _firstAfterSecret
            End Get
            Set(ByVal value As String)
                SetProperty(_firstAfterSecret, value)
            End Set
        End Property
        Public Property WordBetween As String
            Get
                Return _wordBetween
            End Get
            Set(ByVal value As String)
                SetProperty(_wordBetween, value)
            End Set
        End Property
        Public Property LastGuessWasCorrect As Boolean
            Get
                Return _lastGuessWasCorrect
            End Get
            Set(ByVal value As Boolean)
                SetProperty(_lastGuessWasCorrect, value)
            End Set
        End Property
        Public Property GuessedWords As ObservableRangeCollection(Of String)
            Get
                Return _guessedWords
            End Get
            Set(ByVal value As ObservableRangeCollection(Of String))
                SetProperty(_guessedWords, value)
            End Set
        End Property
        Public Property Results As ObservableRangeCollection(Of String)
            Get
                Return _results
            End Get
            Set(ByVal value As ObservableRangeCollection(Of String))
                SetProperty(_results, value)
            End Set
        End Property
        Public Property RemainingWords As ObservableRangeCollection(Of String)
            Get
                Return _remainingWords
            End Get
            Set(ByVal value As ObservableRangeCollection(Of String))
                SetProperty(_remainingWords, value)
            End Set
        End Property
        Public Property NumberCharactersResolved As Integer
            Get
                Return _numberCharactersResolved
            End Get
            Set(ByVal value As Integer)
                _numberCharactersResolved = value
            End Set
        End Property

#End Region

#Region " Constructor                   "

        Public Sub New(ByVal secret As String)
            SecretWord = secret
        End Sub

#End Region

#Region " Methods                       "

        Public Sub AddGuess(ByVal value As String)
            Dim remainingItems As List(Of String)
            Dim lastGuessResult As String = ""

            'fill remaining words if empty
            If RemainingWords.Count = 0 Then RemainingWords.AddRange(AppCommon.WordList)

            'compare word to secret
            Select Case String.Compare(value, SecretWord, StringComparison.OrdinalIgnoreCase)
                Case Is < 0
                    LastGuessWasCorrect = False
                    lastGuessResult = String.Format("Secret word is {0} '{1}'", "after", value)
                Case Is > 0
                    LastGuessWasCorrect = False
                    lastGuessResult = String.Format("Secret word is {0} '{1}'", "before", value)
                Case 0
                    LastGuessWasCorrect = True
                    lastGuessResult = "You guessed the secret word!"
            End Select

            'add result to beginning of collection
            Results.Insert(0, lastGuessResult)

            'add guessed word
            GuessedWords.Add(value)

            'calculate best guesses
            LastBeforeSecret = GuessedWords.Where(Function(i) i.ToLower < SecretWord.ToLower).OrderBy(Function(i) i).DefaultIfEmpty("").LastOrDefault
            FirstAfterSecret = GuessedWords.Where(Function(i) i.ToLower > SecretWord.ToLower).OrderBy(Function(i) i).DefaultIfEmpty("").FirstOrDefault

            WordBetween = GetBestGuesses()

            'calculate words that are still left
            remainingItems = RemainingWords.Where(Function(w) (LastBeforeSecret = "" OrElse w > LastBeforeSecret) AndAlso _
                                                              (FirstAfterSecret = "" OrElse w < FirstAfterSecret)).ToList
            RemainingWords.Clear()
            RemainingWords.AddRange(remainingItems)

        End Sub

        Private Function GetBestGuesses() As String
            Dim beforeWord As String = If(LastBeforeSecret <> "", LastBeforeSecret, "aardvark")
            Dim afterWord As String = If(FirstAfterSecret <> "", FirstAfterSecret, "zymurgy")

            NumberCharactersResolved = SetNumberOfCommonLetters(beforeWord, afterWord)

            If NumberCharactersResolved <> 0 Then
                beforeWord = String.Format("({0}){1}", beforeWord.Substring(0, NumberCharactersResolved), beforeWord.Substring(NumberCharactersResolved))
                afterWord = String.Format("({0}){1}", afterWord.Substring(0, NumberCharactersResolved), afterWord.Substring(NumberCharactersResolved))
            End If

            Return String.Format("Word is between {0} and {1}", beforeWord, afterWord)
        End Function

        Private Function SetNumberOfCommonLetters(ByVal beforeWord As String, ByVal afterWord As String) As Integer
            Dim commonLetters As Integer = 0

            For i = 1 To beforeWord.Length
                If afterWord.ToLower.StartsWith(beforeWord.ToLower.Substring(0, i)) Then
                    commonLetters = i
                Else
                    Exit For
                End If
            Next

            Return commonLetters
        End Function

        Public Sub ResetGuesses()
            LastBeforeSecret = ""
            FirstAfterSecret = ""
            SecretWord = ""
            LastGuessWasCorrect = False
            WordBetween = GetBestGuesses()
            GuessedWords.Clear()
            Results.Clear()
            RemainingWords.Clear()
            RemainingWords.AddRange(AppCommon.WordList)
        End Sub

#End Region

    End Class
End Namespace