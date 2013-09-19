Imports Windows.Storage

Namespace Common

    ''' <summary>
    ''' This Base class provides support for easily saving User Settings with Binable Properties  <br/>
    ''' Call <c>GetProperty</c> in Target Property Getter <br/>
    ''' Call <c>SetProperty</c> in Target Property Setter <br/>
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class SettingsBase : Implements INotifyPropertyChanged

#Region " User Settings Functions       "

        ''' <summary>
        ''' Gets the value of the UserSettings for the passed in Key. <br/>
        ''' Sets default value on first load.
        ''' </summary>
        ''' <typeparam name="T">Type of the property.</typeparam>
        ''' <param name="defaultValue">If Property Key not found, will initialze to this value</param>
        ''' <param name="key">Name of the property Key for value in User Settings. <br/>
        ''' This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        ''' <returns>Property Value</returns>
        ''' <remarks></remarks>
        Protected Function GetProperty(Of T)(defaultValue As T, <CallerMemberName()> Optional key As String = Nothing) As T
            If Not ApplicationData.Current.LocalSettings.Values.ContainsKey(key) Then
                SaveSettings(defaultValue, key)
            End If
            Return ApplicationData.Current.LocalSettings.Values(key)
        End Function

        ''' <summary>
        ''' Get User Settings Property for the specified Key
        ''' </summary>
        ''' <param name="key">Name of the property Key for value in User Settings. <br/>
        ''' This value is optional and can be provided automatically when invoked from compilers 
        ''' that support <see cref="CallerMemberNameAttribute"/>.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetSettingsValue(<CallerMemberName()> Optional key As String = Nothing) As Object
            Return ApplicationData.Current.LocalSettings.Values(key)
        End Function

        ''' <summary>
        ''' Saves the passed in value
        ''' </summary>
        ''' <typeparam name="T">Type of the property.</typeparam>
        ''' <param name="value">Desired value for the property.</param>
        ''' <param name="key">Name of the property Key for value in User Settings. <br/>
        ''' This value is optional and can be provided automatically when invoked from compilers 
        ''' that support <see cref="CallerMemberNameAttribute"/>.</param>
        ''' <remarks></remarks>
        Private Sub SaveSettings(Of T)(value As T, <CallerMemberName()> Optional key As String = Nothing)
            ApplicationData.Current.LocalSettings.Values(key) = value
        End Sub

        Private Function LoadSettings(key As String) As Object
            Return ApplicationData.Current.LocalSettings.Values(key)
        End Function

#End Region

#Region " INotify Property Changed      "

        ''' <summary>
        ''' Multicast event for property change notifications.
        ''' </summary>
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Checks if a property already matches a desired value.  <br/>
        ''' Sets the property and notifies listeners only when necessary.
        ''' </summary>
        ''' <typeparam name="T">Type of the property.</typeparam>
        ''' <param name="value">Desired value for the property.</param>
        ''' <param name="propertyName">Name of the property used to notify listeners.  This value
        ''' is optional and can be provided automatically when invoked from compilers that support
        ''' <see cref="CallerMemberNameAttribute"/>.</param>
        Protected Sub SetProperty(Of T)(value As T, <CallerMemberName> Optional propertyName As String = Nothing)
            If GetSettingsValue(propertyName).Equals(value) Then Return
            SaveSettings(value, propertyName)
            OnPropertyChanged(propertyName)
        End Sub

        ''' <summary>
        ''' Notifies listeners that a property value has changed.
        ''' </summary>
        ''' <param name="propertyName">Name of the property used to notify listeners.  This value
        ''' is optional and can be provided automatically when invoked from compilers that support
        ''' <see cref="CallerMemberNameAttribute"/>.</param>
        Private Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

#End Region

    End Class

End Namespace