// Copyright 2017, Google Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using Grpc.Core;
using System;
using System.Windows.Forms;
using Com.Example.Grpc.Chat;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ChatWindowsClient
{
    public partial class ChatForm : Form
    {
        private const string Host = "localhost";
        private const int Port = 8080;

        private AsyncDuplexStreamingCall<ChatMessage, ChatMessageFromServer> _call;

        public ChatForm()
        {
            InitializeComponent();
            InitializeGrpc();
        }

        private void InitializeGrpc()
        {
            var channel = new Channel(Host + ":" + Port, ChannelCredentials.Insecure);

            // Create a client with the channel
            var chatService = new ChatService.ChatServiceClient(channel);

            // Open a connection to server
            _call = chatService.chat();

        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                return;
            }

            // Create a message
            var message = new ChatMessage
            {
                From = nameTextBox.Text,
                Message = messageTextBox.Text
            };

            // Didable changing name.
            nameTextBox.Enabled = false;
            messageTextBox.Text = "";

            try
            {

                // Send the message
                await _call.RequestStream.WriteAsync(message);
                Debug.WriteLine("WriteAsync complete");
            }
            catch (RpcException ex)
            {
                Debug.WriteLine($"RpcException {ex}");
            }
        }

        // Async task. Stay in "backgroud".
        private async void ChatForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Wait for results from server
                // When connection is closed, the loop exits.
                while (await _call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var serverMessage = _call.ResponseStream.Current;
                    var otherClientMessage = serverMessage.Message;
                    var displayMessage = string.Format("{0}:{1}{2}", otherClientMessage.From, otherClientMessage.Message, Environment.NewLine);
                    chatTextBox.Text += displayMessage;
                }
            }
            catch (RpcException ex)
            {
                Debug.WriteLine($"RpcException {ex}");
                MessageBox.Show($"ResponseStream received exception\r\n{ex}");
                this.Close();
            }

            Debug.WriteLine("ChatForm_Load async stopped");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Close connection
            var task = _call.RequestStream.CompleteAsync();
            Debug.WriteLine("Wait completeAsync");
            task.Wait();
            Debug.WriteLine("completeAsync complete");

            base.OnClosing(e);
        }
    }
}
