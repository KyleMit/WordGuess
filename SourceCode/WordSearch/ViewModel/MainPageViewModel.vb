Imports Windows.UI
Imports WinRTXamlToolkit.Controls
Imports GalaSoft.MvvmLight.Command
Imports Windows.UI.Popups
Imports WordSearch.Model
Imports Windows.Storage.Streams
Imports Windows.Storage

Namespace ViewModel

    Public Class MainPageViewModel : Inherits Common.BindableBase

#Region " Private Variables             "

        Private ReadOnly mySettings As New Resources.UserSettings

        'backing properties

        Private _secretWord As String = ""
        Private _guesses As New GuessedWords("")
        Private _currentGuess As String
        Private _isWordInputFocused As Boolean
        Private _secretWordIsSet As Boolean

        Private ReadOnly _enterSecretWordPrompt As String = GetEnterSecretWordPrompt()

#End Region

#Region " Public Properties             "

        Public Property CurrentGuess() As String
            Get
                Return _currentGuess
            End Get
            Set(ByVal value As String)
                SetProperty(_currentGuess, value)
            End Set
        End Property
        Public Property SecretWord() As String
            Get
                Return _secretWord
            End Get
            Set(ByVal value As String)
                'value is not set when equal to nothing
                SecretWordIsSet = value <> ""
                SetProperty(_secretWord, value)
            End Set
        End Property
        Public Property IsWordInputFocused() As Boolean
            Get
                Return _isWordInputFocused
            End Get
            Set(ByVal value As Boolean)
                SetProperty(_isWordInputFocused, value)
            End Set
        End Property
        Public Property Guesses As GuessedWords
            Get
                Return _guesses
            End Get
            Set(ByVal value As GuessedWords)
                SetProperty(_guesses, value)
            End Set
        End Property
        Public Property SecretWordIsSet As Boolean
            Get
                Return _secretWordIsSet
            End Get
            Set(ByVal value As Boolean)
                SetProperty(_secretWordIsSet, value)
            End Set
        End Property
        Public ReadOnly Property EnterSecretWordPrompt() As String
            Get
                Return _enterSecretWordPrompt
            End Get
        End Property

#End Region

#Region " Command Properties            "

        Private _loadWordsCommand As RelayCommand
        Public ReadOnly Property LoadWordsCommand As RelayCommand
            Get
                If _loadWordsCommand Is Nothing Then
                    _loadWordsCommand = New RelayCommand(Async Sub() Await LoadWords())
                End If
                Return _loadWordsCommand
            End Get
        End Property

        Private _wordSubmitCommand As RelayCommand
        Public ReadOnly Property WordSubmitCommand() As RelayCommand
            Get
                If _wordSubmitCommand Is Nothing Then
                    _wordSubmitCommand = New RelayCommand(AddressOf WordSubmit)
                End If
                Return _wordSubmitCommand
            End Get
        End Property

        Private _enterSecretWordCommand As RelayCommand
        Public ReadOnly Property EnterSecretWordCommand As RelayCommand
            Get
                If _enterSecretWordCommand Is Nothing Then
                    _enterSecretWordCommand = New RelayCommand(Async Sub() Await EnterSecretWord())
                End If
                Return _enterSecretWordCommand
            End Get
        End Property

        Private _generateSecretWordCommand As RelayCommand
        Public ReadOnly Property GenerateSecretWordCommand As RelayCommand
            Get
                If _generateSecretWordCommand Is Nothing Then
                    _generateSecretWordCommand = New RelayCommand(Async Sub() Await GenerateSecretWord())
                End If
                Return _generateSecretWordCommand
            End Get
        End Property

        Private _showHintCommand As RelayCommand
        Public ReadOnly Property ShowHintCommand As RelayCommand
            Get
                If _showHintCommand Is Nothing Then
                    _showHintCommand = New RelayCommand(Async Sub() Await ShowHint())
                End If
                Return _showHintCommand
            End Get
        End Property


        Private _giveUpCommand As RelayCommand
        Public ReadOnly Property GiveUpCommand As RelayCommand
            Get
                If _giveUpCommand Is Nothing Then
                    _giveUpCommand = New RelayCommand(Async Sub() Await GiveUp())
                End If
                Return _giveUpCommand
            End Get
        End Property

        Private _resetCommand As RelayCommand
        Public ReadOnly Property ResetCommand As RelayCommand
            Get
                If _resetCommand Is Nothing Then
                    _resetCommand = New RelayCommand(Sub() Reset())
                End If
                Return _resetCommand
            End Get
        End Property

#End Region

#Region " Load Methods                  "

        Private Async Function LoadWords() As Task
            If AppCommon.WordList Is Nothing Then
                AppCommon.WordList = Await GetWords()
            End If
        End Function

        Private Async Function GetWords() As Task(Of List(Of String))
            Const folderName As String = "AppData"
            Const fileName As String = "WordListComma.txt"
            Dim dataFolder As StorageFolder
            Dim dataFile As StorageFile
            Dim fileContent As String

            dataFolder = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folderName)
            dataFile = Await dataFolder.GetFileAsync(fileName)

            If dataFile Is Nothing Then Throw New Exception(String.Format("Could not find file {0}.", fileName))

            fileContent = Await ReadFileAsString(dataFile)

            Return fileContent.Split(",").AsEnumerable.ToList()

        End Function

        Private Async Function ReadFileAsString(ByVal file As StorageFile) As Task(Of String)
            Dim numBytesLoaded As UInt32
            Dim size As UInt64
            Dim fileContent As String = ""

            Using readStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                Using dataReader As New DataReader(readStream)
                    size = readStream.Size
                    If size <= UInt32.MaxValue Then
                        numBytesLoaded = Await dataReader.LoadAsync(CType(size, UInt32))
                        fileContent = dataReader.ReadString(numBytesLoaded)
                    End If
                End Using
            End Using

            Return fileContent
        End Function

#End Region

#Region " Page Methods                  "

        Private Async Sub WordSubmit()
            Dim message As String
            Dim messageDialog As MessageDialog

            Try

                'cancel if secret word not set
                If SecretWord = "" Then
                    message = "Secret word not set.  Please enter or generate one."
                    messageDialog = New MessageDialog(message)
                    Await messageDialog.ShowAsync()
                    Exit Sub
                End If

                'cancel if word not entered
                If CurrentGuess = "" Then Exit Sub

                'cancel if not in dictionary
                If mySettings.EnforceSpellcheck AndAlso
                   Not AppCommon.WordList.Any(Function(item) item = CurrentGuess.Trim.ToLower) Then
                    message = String.Format("Could not find {0} in dictionary", CurrentGuess.Trim.ToLower)
                    messageDialog = New MessageDialog(message)
                    Await messageDialog.ShowAsync()
                    Exit Sub
                End If

                Guesses.AddGuess(CurrentGuess)

                'let user know they were correct
                If Guesses.LastGuessWasCorrect Then
                    messageDialog = New MessageDialog(Guesses.Results.Last())
                    Await messageDialog.ShowAsync()
                End If


            Catch ex As Exception

            Finally
                'Reset Word Entry
                CurrentGuess = ""
                IsWordInputFocused = True

            End Try
        End Sub

#End Region

#Region " App Bar Commands              "

        Private Async Function EnterSecretWord() As Task
            Dim dialog As New InputDialog()

            Dim btnStyle As New Style(GetType(Button))
            btnStyle.Setters.Add(New Setter(Button.BackgroundProperty, Color.FromArgb(255, 139, 0, 0)))
            dialog.ButtonStyle = btnStyle
            dialog.Background = New SolidColorBrush(Color.FromArgb(255, 139, 0, 0))

            Dim result = Await dialog.ShowAsync("Enter New Word", "Set new secret word:", "Save", "Cancel")
            If result = "Save" AndAlso dialog.InputText <> "" Then
                Await SetWord(dialog.InputText.ToLower)
            End If
        End Function

        Private Async Function GenerateSecretWord() As Task
            Dim rand As New Random
            Await SetWord(AppCommon.WordList(rand.Next(0, AppCommon.WordList.Count)))
        End Function

        Private Async Function SetWord(ByVal newWord As String) As Task
            Dim messageDialog As MessageDialog
            Dim message As String

            'check spelling
            If AppCommon.WordList.Any(Function(s) s = newWord) Then
                message = "Secret Word Set"
                'reset controls
                Guesses.ResetGuesses()
                Guesses.SecretWord = newWord
                SecretWord = newWord
            Else
                message = String.Format("Could not find word '{0}' in dictionary.", newWord)
            End If

            'tell user what's up
            messageDialog = New MessageDialog(message)
            Await messageDialog.ShowAsync()

        End Function

        Private Async Function ShowHint() As Task
            Dim message As String
            Dim nextLetter As String = "nothing"
            Dim messageDialog As MessageDialog

            'exit if secret not yet set
            If SecretWord = "" Then
                message = "You haven't picked a word yet"
                messageDialog = New MessageDialog(message)
                Await messageDialog.ShowAsync()
                Exit Function
            End If

            'determine next letter
            If Guesses.SecretWord.Length >= Guesses.NumberCharactersResolved + 1 Then
                nextLetter = Guesses.SecretWord.Substring(Guesses.NumberCharactersResolved, 1)
            End If

            message = String.Format("Next letter of Secret Word is {0}", nextLetter)
            messageDialog = New MessageDialog(message)
            Await messageDialog.ShowAsync()
        End Function

        Private Async Function GiveUp() As Task
            Dim message As String
            Dim messageDialog As MessageDialog

            If SecretWord = "" Then
                message = "You haven't picked a word yet on which you could give up"
                messageDialog = New MessageDialog(message)
                Await messageDialog.ShowAsync()
                Exit Function
            End If

            message = String.Format("Secret Word was {0}", Guesses.SecretWord)
            messageDialog = New MessageDialog(message)
            Await messageDialog.ShowAsync()

            Reset()

        End Function

        Private Sub Reset()
            SecretWord = ""
            Guesses.ResetGuesses()
        End Sub

        Private Function GetEnterSecretWordPrompt() As String
            Dim capabilities As New Windows.Devices.Input.TouchCapabilities()
            Dim hasTouch As Boolean = capabilities.TouchPresent <> 0
            Dim message As String = If(hasTouch, "Swipe Up ", "Right Click Anywhere ") & "to Open App Bar and Set Secret Word"
            Return message
        End Function

#End Region

    End Class

End Namespace