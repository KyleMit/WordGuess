Imports WordSearch.ViewModel
Imports Windows.Storage.Streams
Imports Windows.Storage
Imports WordSearch.Model

Namespace View

    Public NotInheritable Class Main : Inherits Common.LayoutAwarePage

#Region " Initializations               "

        ''' <summary>
        ''' Invoked when this page is about to be displayed in a Frame.
        ''' </summary>
        ''' <param name="e">Event data that describes how this page was reached.  The Parameter
        ''' property is typically used to configure the page.</param>
        Protected Overrides Sub OnNavigatedTo(e As Navigation.NavigationEventArgs)
            'get view model and submit command
            Dim viewModel = CType(DataContext, MainPageViewModel)
            viewModel.LoadWordsCommand.Execute(Nothing)
        End Sub

#End Region

#Region " State Management              "

        ''' <summary>
        ''' Populates the page with content passed during navigation.  Any saved state is also
        ''' provided when recreating a page from a prior session.
        ''' </summary>
        ''' <param name="navigationParameter">The parameter value passed to
        ''' <see cref="Frame.Navigate"/> when this page was initially requested.
        ''' </param>
        ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
        ''' session.  This will be null the first time a page is visited.</param>
        Protected Overrides Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))

        End Sub

        ''' <summary>
        ''' Preserves state associated with this page in case the application is suspended or the
        ''' page is discarded from the navigation cache.  Values must conform to the serialization
        ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
        ''' </summary>
        ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))

        End Sub

#End Region

#Region " Page Events                   "

        Private Sub KeyDownEntered(sender As Object, e As KeyRoutedEventArgs) Handles pageRoot.KeyDown
            'check if enter key hit
            If e.Key = Windows.System.VirtualKey.Enter Then
                'get view model and submit command
                Dim viewModel = CType(DataContext, MainPageViewModel)
                viewModel.WordSubmitCommand.Execute(Nothing)
                e.Handled = True
            End If
        End Sub

#End Region

    End Class
End Namespace