Namespace Common

    ''' <summary>A class that allows you to chain multiple IValueConverters</summary>
    ''' <remarks>http://stackoverflow.com/a/3130887/1366033</remarks>
    Public Class ValueConverterGroup : Inherits List(Of IValueConverter) : Implements IValueConverter

#Region "IValueConverter Members"

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Return Aggregate(value, Function(current, converter) converter.Convert(current, targetType, parameter, language))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function

#End Region

    End Class
End Namespace