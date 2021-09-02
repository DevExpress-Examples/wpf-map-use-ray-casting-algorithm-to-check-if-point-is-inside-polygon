Imports DevExpress.Xpf.Map
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Namespace DXMapShapeFile1
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub vLayer_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			Dim point As GeoPoint = CType(mapControl.ScreenPointToCoordPoint(e.GetPosition(mapControl)), GeoPoint)
			Dim hi = mapControl.CalcHitInfo(e.GetPosition(mapControl))
			For Each item In vectorLayer.Data.DisplayItems
				If IsPointInsideItem(item, point) Then
					Me.Title = item.Attributes("NAME").Value.ToString()
				End If
			Next item
		End Sub

		Private Function IsPointInsideItem(ByVal item As MapItem, ByVal point As GeoPoint) As Boolean
			Dim polygon As MapPolygon = TryCast(item, MapPolygon)
			If polygon IsNot Nothing Then
				Return IsPointInside(polygon.Points.Cast(Of GeoPoint)().ToArray(), point)
			End If
			Dim path As MapPath = TryCast(item, MapPath)
			If path IsNot Nothing Then
				Dim segments = CType(path.Data, MapPathGeometry).Figures.SelectMany(Function(f) f.Segments).Cast(Of MapPolyLineSegment)()
				Return segments.Any(Function(segment) IsPointInside(segment.Points.Cast(Of GeoPoint)().ToArray(), point))

			End If
			Return False
		End Function

		Private Function IsPointInside(ByVal polygon() As GeoPoint, ByVal testPoint As GeoPoint) As Boolean
			Dim result As Boolean = False
			Dim j As Integer = polygon.Count() - 1
			For i As Integer = 0 To polygon.Count() - 1
				If polygon(i).Latitude < testPoint.Latitude AndAlso polygon(j).Latitude >= testPoint.Latitude OrElse polygon(j).Latitude < testPoint.Latitude AndAlso polygon(i).Latitude >= testPoint.Latitude Then
'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
					If polygon(i).Longitude + (testPoint.Latitude - polygon(i).Latitude) / (polygon(j).Latitude - polygon(i).Latitude) * (polygon(j).Longitude - polygon(i).Longitude) < testPoint.Longitude Then
						result = Not result
					End If
				End If
				j = i
			Next i
			Return result
		End Function



	End Class
End Namespace
