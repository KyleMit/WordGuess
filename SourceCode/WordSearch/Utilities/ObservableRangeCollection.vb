Imports System.Collections.Specialized

Namespace Utilities

    ''' <summary>Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.</summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>http://stackoverflow.com/a/670579/1366033</remarks>
    Public Class ObservableRangeCollection(Of T) : Inherits ObservableCollection(Of T)

        ''' <summary><b>Adds</b> the elements of the specified collection to the end of the <see cref="ObservableCollection(Of T)"/>.</summary>
        ''' <param name="newItems">Range of <see cref="IEnumerable(Of T)"/> items to add to collection</param>
        ''' <remarks></remarks>
        Public Sub AddRange(newItems As IEnumerable(Of T))
            CheckReentrancy()

            For Each newItem As T In newItems
                Items.Add(newItem)
            Next

            ' Now raise the changed events
            OnPropertyChanged(New PropertyChangedEventArgs("Count"))
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))


            'OnPropertyChanged(New PropertyChangedEventArgs("Item()"))
            'OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems.ToList, startingIndex))

        End Sub

        ''' <summary><b>Removes</b> the first occurecnce of each item in the specified collection from <see cref="ObservableCollection(Of T)"/>.</summary>
        ''' <param name="newItems">Range of <see cref="IEnumerable(Of T)"/> items to removes from collection</param>
        ''' <remarks></remarks>
        Public Sub RemoveRange(newItems As IEnumerable(Of T))
            CheckReentrancy()
            Dim newList = newItems.OrderBy(Function(s) s).ToList
            For Each newItem As T In newList
                Items.Remove(newItem)
            Next

            ' Now raise the changed events
            OnPropertyChanged(New PropertyChangedEventArgs("Count"))
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
        End Sub

    End Class

End Namespace