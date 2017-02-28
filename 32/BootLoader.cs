using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Hologic.CANRouter;
using Peak.Can.Light;

namespace ICLRead
{
    public partial class BootLoader : Form
    {
        #region Variables

        ConsoleForm console;

        MessageWriter writer;

        MsgToResMethods M2R;

        static List<uint> nodes = new List<uint>();
        static int selectedIndex = 0;

        public static List<TCLightMsg> ParsedMsgs;

        /// <summary>
        /// Used to get next position of ParsedMsgs
        /// </summary>
        private static int index = -1;

        #endregion

        #region Constructor(s)

        public BootLoader( ConsoleForm console)
        {
            InitializeComponent();
            ModuleStrSelectBox.SelectedIndex = 0;
            this.console = console;
            writer = new MessageWriter(console);
            M2R = new MsgToResMethods(console);
            fillNodesDict();
            selectedIndex = ModuleStrSelectBox.SelectedIndex;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns an enumeration of Messages to be sent
        /// </summary>
        private IEnumerable<TCLightMsg> DownloadParser()
        {
            

            #region Declarations

            StreamReader DwnldFile;

            try
            {
                DwnldFile = new StreamReader(openFileDialog1.FileName);
            }
            catch (ArgumentException)
            {
                yield break;
            }

            #region FileValCheck

            if (DwnldFile.Peek() == -1)
            {
                yield break;
            }

            #endregion

            //TCLightMsg SendMsg = writer.BuildCanBlkSendSeg();

            TCLightMsg StartMsg = writer.BuildMessage((uint)MSG.CAN_BTLD_SVCE_START,
                                                      (uint)PID.CAN_PID_COMMAND,
                                                      GetDID(),
                                                      (uint)SID.CAN_SID_EXT);

            yield return StartMsg;

            String segment;

            #endregion

            while ((segment = DwnldFile.ReadLine()) != null)//for each line/segment
            {
                //yield return SendMsg;

                #region Description Message

                TCLightMsg DescMsg = writer.BuildMessage((uint)MSG.CAN_BLK_SEG_DESC,
                                                     (uint)PID.CAN_PID_OTHER,
                                                     GetDID(),
                                                     (uint)SID.CAN_SID_EXT);
                
                DescMsg.Data[0] = Byte.Parse("" + segment.ElementAt(1));
                DescMsg.Data[2] = Byte.Parse("" + segment.ElementAt(2) +
                                                  segment.ElementAt(3),
                                                  System.Globalization.NumberStyles.AllowHexSpecifier);


                //3 and 4 here.  change 5 and 6 maybe.  Change start position

                // 1 == 2-bytes, 2 == 3-bytes, 3 == 4-bytes

                //if desc[0] == 1, 2, or 3
                if (DescMsg.Data[0] == 1 || DescMsg.Data[0] == 2 || DescMsg.Data[0] == 3)
                {
                    //for each byte
                    for (int i = 0; i < DescMsg.Data[0] + 1; i++)
                    {
                        DescMsg.Data[DescMsg.Data.Length - 2 - i] = Byte.Parse("" + segment.ElementAt(4 + ((DescMsg.Data[0] - i) * 2)) +
                                                      segment.ElementAt(5 + ((DescMsg.Data[0] - i) * 2)),
                                                      System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                }


                //DescMsg.Data[5] = Byte.Parse("" + segment.ElementAt(4) +
                //                                  segment.ElementAt(5),
                //                                  System.Globalization.NumberStyles.AllowHexSpecifier);
                //DescMsg.Data[6] = Byte.Parse("" + segment.ElementAt(6) +
                //                                  segment.ElementAt(7),
                //                                  System.Globalization.NumberStyles.AllowHexSpecifier);
                DescMsg.Data[7] = Byte.Parse("" + segment.ElementAt(segment.Length - 2) +
                                                  segment.ElementAt(segment.Length - 1),//Length -1 == last index
                                                  System.Globalization.NumberStyles.AllowHexSpecifier);

                yield return DescMsg;

                #endregion

                #region Data Message

                int position = 0;

                for (byte messageNum = 0; messageNum < (byte)Math.Ceiling(((decimal)(DescMsg.Data[2] - 3)/6)) ; messageNum++)//for each message
                {
                    TCLightMsg DataMsg = writer.BuildMessage((uint)MSG.CAN_BLK_SEG_DATA,
                                                 (uint)PID.CAN_PID_OTHER,
                                                 GetDID(),
                                                 (uint)SID.CAN_SID_EXT);

                    for (byte byteNum = 0; byteNum < 6; byteNum++)//for each byte in message
                    {
                        //the "* 2" is so that we record a byte then move one byte (instead of one character, a half byte) each iteration
                        //in other words, the "* 2" is so that we operate in bytes, not characters
                        position = (3 + DescMsg.Data[0] + messageNum * 6 + byteNum) * 2;//DescMsg.Data[0] = number of bytes stolen by DescMsg for the address

                        if ( position < segment.Length - 2 )//clips off the Checksum from the DataMsg (stolen by the DescMsg)
                        {
                            DataMsg.Data[2 + byteNum] = Byte.Parse(segment.ElementAt(position).ToString() +
                                                             segment.ElementAt(1 + position).ToString(),
                                                             System.Globalization.NumberStyles.AllowHexSpecifier);
                        }
                        else if (DataMsg.Data[0] == (DataMsg.Data[1]))//if last of seg
                        {
                            if (DescMsg.Data[0] > 3)
                            {
                                DataMsg.Len = (byte)(2 + ((DescMsg.Data[2] - 3) % 6));
                            }
                            //total len minus the bytes stolen by DescMsg, add two (the first two bytes of data are always used for size), all minus ("total divided by 6" floored) x 6 (gets difference ) = len
                            DataMsg.Len = (byte)(2 + ((DescMsg.Data[2] - DescMsg.Data[0] - 2) % 6));// - ((int)Math.Floor(((decimal)DescMsg.Data[2] - 3) / 6) * 6));
                        }

                        #region CASE: End of Line

                        //if (i == DescMsg.Data[2] - 4)//OR if Last
                        //{
                        //    DataMsg.Data[0] = (byte)Math.Ceiling(((decimal)i));
                        //    DataMsg.Data[1] = (byte)Math.Ceiling(((decimal)DescMsg.Data[2] - 3) / 6);

                        //    if (DataMsg.Data[0] != DataMsg.Data[1])
                        //    {
                        //        //console.AddMiscMsg("Error: Improper Format.  Reached end of line and Lengths do not match (Data[0] and Data[1] are not equal).");
                        //    }

                        //    yield return DataMsg;
                        //    //console.AddMiscMsg("wwwwwwwwwwwwwwwwwwwwwwwwwwww");
                        //}

                        #endregion
                    }

                    #region Full Message

                    if (DescMsg.Data[0] == 1 || DescMsg.Data[0] == 2 || DescMsg.Data[0] == 3)
                    {
                        DataMsg.Data[0] = (byte)(messageNum + 1);
                        DataMsg.Data[1] = (byte)Math.Ceiling(((decimal)DescMsg.Data[2] - DescMsg.Data[0] - 2) / 6);

                        yield return DataMsg;
                    }
                    else if (DescMsg.Data[0] == 0)
                    {

                    }
                    else if (DescMsg.Data[0] > 3)
                    {
                        DataMsg.Data[0] = (byte)(messageNum + 1);
                        DataMsg.Data[1] = (byte)Math.Ceiling(((decimal)DescMsg.Data[2] - 3) / 6);
                        DataMsg.Data[2] = DescMsg.Data[0];

                        yield return DataMsg;
                    }
                    #endregion
                }

                #endregion
            }
            yield break;
        }

        /// <summary>
        /// Fills the List with values in matching order with the selectbox
        /// nodes(ModuleStrSelectBox.SelectedIndex) =-> current nodes DID
        /// </summary>
        private void fillNodesDict()
        {
            foreach (string q in ModuleStrSelectBox.Items)
            {
                nodes.Add((uint)Enum.Parse(typeof(DID), "CAN_DID_" + q));
            }
        }

        #region Public Called

        public static TCLightMsg GetMsg()
        {
            try
            {
                return ParsedMsgs[++index];
            }
            catch 
            {
                TCLightMsg msg = new TCLightMsg();
                msg.ID = 0;
                msg.Data[0] = 0;
                msg.MsgType = MsgTypes.MSGTYPE_EXTENDED;
                msg.Len = 8;
                return msg;
            }
        }

        public static uint GetDID()
        {
            return nodes[selectedIndex];
        }

        #endregion

        #endregion

        #region Form Functions

        private void OpenFileBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //do thing
                CloseFileBtn.Enabled = true;
                FilePathTb.Text = openFileDialog1.FileName;
                FileSizeTb.Text = new System.IO.FileInfo( openFileDialog1.FileName ).Length.ToString();
            }
        }

        private void CloseFileBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Reset();
            CloseFileBtn.Enabled = false;
            FilePathTb.Text = "";
            FileSizeTb.Text = "";
        }

        private void NPTModeBtn_Click(object sender, EventArgs e)
        {
            #region Declarations

            TCLightMsg NPTMsg = writer.BuildMessage((uint)MSG.CAN_SET_NODE_OPMODE, 
                                (uint)PID.CAN_PID_COMMAND, 
                                GetDID(), 
                                (uint)SID.CAN_SID_EXT);

            #endregion

            #region Data

            NPTMsg.Data[0] = 0x01;//Mode to enter

            NPTMsg.Data[1] = (byte)DateTime.Now.Month;
            NPTMsg.Data[2] = (byte)DateTime.Now.Day;
            NPTMsg.Data[3] = (byte)(DateTime.Now.Year-2000);
            NPTMsg.Data[4] = (byte)DateTime.Now.Hour;
            NPTMsg.Data[5] = (byte)DateTime.Now.Minute;
            NPTMsg.Data[6] = (byte)DateTime.Now.Second;

            #endregion

            #region Write

            writer.WriteMessage(NPTMsg);

            #endregion
        }

        private void JumpBtLdrBtn_Click(object sender, EventArgs e)
        {
            #region Declarations

            TCLightMsg BtLdJmpMsg = writer.BuildMessage((uint)MSG.CAN_BTLD_JMP_TO_BOOTLDR,
                                (uint)PID.CAN_PID_COMMAND,
                                GetDID(),
                                (uint)SID.CAN_SID_EXT);
            //TCLightMsg DnldJmpMsg = writer.BuildMessage((uint)MSG.CAN_DNLD_JMP_TO_BOOTLDR,
            //                    (uint)PID.CAN_PID_COMMAND,
            //                    (uint)DID.CAN_DID_THD,
            //                    (uint)SID.CAN_SID_EXT);

            #endregion

            #region Write

            writer.WriteMessage(BtLdJmpMsg);

            //writer.WriteMessage(DnldJmpMsg, writer.BuildMessageTime());

            #endregion
        }

        /// <summary>
        /// Gets an enumeration of messages to be sent and sends them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartDownloadBtn_Click(object sender, EventArgs e)
        {
            StartDownloadBtn.Enabled = false;
            ParsedMsgs = DownloadParser().ToList();//ehhhhh... Well, it gets the messages into List Format
            console.AddMsgToResponse((uint)MSG.CAN_BTLD_SVCE_START, M2R.SendStartResponse);//Makes it so that we can pick up the response message
            writer.WriteMessage(GetMsg());
            StartDownloadBtn.Enabled = true;
        }

        private void EnableDownloadButton()
        {
            if (StartDownloadBtn.InvokeRequired)
            {
                StartDownloadBtn.BeginInvoke(new Action(delegate { EnableDownloadButton(); }));
            }
            else
            {
                //StartDownloadBtn.Enabled = true;
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ModuleStrSelectBox_SelectedItemChanged(object sender, EventArgs e)
        {
            selectedIndex = ModuleStrSelectBox.SelectedIndex;
        }

        #endregion
    }
}
