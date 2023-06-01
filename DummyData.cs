namespace charposition;

internal static class DummyData
{
    public const string Title = "Sample code";

    public const string Text =
@"class Socket
{
   void Connect(string server)
   {
      SocketLibrary.Connect(mSocket, server);
   }

   void Disconnect()
   {
      SocketLibrary.Disconnect(mSocket);
   }
}";

    public const string Semantics =
@"---
type: file
name: FooSocket.csharp
locationSpan : {start: [1, 0], end: [12, 1]}
footerSpan : [0,-1]
parsingErrorsDetected : false
children:

  - type : class
    name : Socket
    locationSpan : {start: [1, 0], end: [12, 1]}
    headerSpan : [0, 16]
    footerSpan : [186, 186]
    children :

    - type : method
      name : Connect
      locationSpan : {start: [3, 0], end: [7,2]}
      span : [17, 109]

    - type : method
      name : Disconnect
      locationSpan : {start: [8,0], end: [11,6]}
      span : [110, 185]";
}
