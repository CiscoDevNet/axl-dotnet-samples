using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using System.Xml.Linq;

// Class that implements SoapCore IEndpiontBehavior and IClientMessageInspector
// to access and print SOAP request and response

namespace Debug
{
    public class DebugEndpointBehaviour : IEndpointBehavior
    {
        public void AddBindingParameters( ServiceEndpoint endpoint, BindingParameterCollection bindingParameters ) { }

        public void ApplyClientBehavior( ServiceEndpoint endpoint, ClientRuntime clientRuntime ) 
        {
            clientRuntime.ClientMessageInspectors.Add( new DebugMessageInspector() );
        }

        public void ApplyDispatchBehavior( ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher ) { }

        public void Validate( ServiceEndpoint endpoint ) { }
    }

    public class DebugMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest( ref Message request, IClientChannel channel )
        {
            using ( var buffer = request.CreateBufferedCopy( int.MaxValue ) )
            {
                var xml = GetBody( buffer.CreateMessage() );
                System.Console.WriteLine( $"Request\n-------\nBody:\n{ xml }\n" );

                request = buffer.CreateMessage();

                return null;
            }
        }

        public void AfterReceiveReply( ref Message reply, object correlationState )
        {
            // Message type can only be read once!  Copy it to memory...
            using ( var buffer = reply.CreateBufferedCopy( int.MaxValue ) )
            {
                // Use the copy to create a message object and process it in GetBody
                var xml = GetBody( buffer.CreateMessage() );
                System.Console.WriteLine( $"Response\n-------\nBody:\n{ xml }\n" );

                // Restore the original message from the copy, for any downstream processing
                reply = buffer.CreateMessage();
            }
        }

        private String GetBody( Message request )
        {
            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                // Pretty print with indents
                var settings = new XmlWriterSettings();
                settings.Indent = true;

                // write request to memory stream
                XmlWriter writer = XmlWriter.Create( memoryStream, settings );
                request.WriteMessage( writer );
                writer.Flush();
                memoryStream.Position = 0;

                // Encode the memory stream as a string and return
                return Encoding.UTF8.GetString( memoryStream.ToArray() );
            }
        }

    }
}

