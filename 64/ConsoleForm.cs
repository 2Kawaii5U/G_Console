using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Data;
using Peak.Can.Light;
using System.Threading;
using Hologic.CANRouter;
using System.Timers;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace ICLRead
{
	/// <summary>
	/// Main Form, Has CAN Message Viewer, Node Viewer, Controls, and Navigation
	/// </summary>
	public partial class ConsoleForm : Form
    {
        #region Variables

        #region Structures
        /// <summary>
        /// Stores Msg and Timestamp values in a struct,
        /// for use in a queue
        /// </summary>
        private struct MsgTime
        {
            TCLightMsg MyMsg;
            TCLightTimestamp MyTimeStamp;

            public MsgTime(TCLightMsg Msg, TCLightTimestamp TimeStamp)
            {
                MyMsg = Msg;
                MyTimeStamp = TimeStamp;
            }

            public TCLightMsg CANMessage
            {
                get { return MyMsg; }
            }

            public TCLightTimestamp Timestamp
            {
                get { return MyTimeStamp; }
            }
        }

        private struct MSGListVal
        {
            MSG message;
            uint mid;

            public MSGListVal(MSG message, uint mid)
            {
                this.message = message;
                this.mid = mid;
            }

            public MSG Message
            {
                get { return message; }
            }

            public uint MID
            {
                get { return mid; }
            }
        }
		#endregion

        #region Private Variables

        #region LastofNode

        /// <summary>
        /// Stores the last Data points, per node, for reference 
        /// </summary>
        private uint LastAWS = 0;
        private uint LastDET = 0;
        private uint LastDTC = 0;
        private uint LastGCB = 0;
        private uint LastPMC = 0;
        private uint LastGEN = 0;
        private uint LastVTA = 0;
        private uint LastCRM = 0;
        private uint LastTHD = 0;
        private uint LastCDI = 0;
        private uint LastAIO = 0;
        private uint LastBKY = 0;
        private uint LastBGM = 0;
        private uint LastBCM = 0;
        private uint LastTCB = 0;
        private uint LastACC = 0;

        #endregion

        #region MaxMsgLen
        /// <summary>
        /// Default is Minimum Message Length + 1 (to account for the space)
        /// </summary>
        int MaxTIDLen = 5;
        int MaxGIDLen = 4;
        int MaxMIDLen = 9;
        int MaxDIDLen = 5;
        int MaxSIDLen = 3;

        #endregion

        public MsgToResMethods M2R;

        /// <summary>
        /// Allows the calling of a method via a message ID
        /// Used to Facilitate responses
        /// </summary>
        private static Dictionary<uint, Action<TCLightMsg>> MsgToResponse = new Dictionary<uint, Action<TCLightMsg>>();

        /// <summary>
        /// The List of Messages, referenced by Group ID
        /// Populated by GetMessagesDividedByGroup(), called either on Init, or on CAN Connect
        /// </summary>
        private Dictionary<uint, List<MSGListVal>> MsgsDividedByGroup = null;

        #region Forms

        /// <summary>
        /// represents the active sub form
        /// </summary>
        public Form activeForm;

        /// <summary>
        /// The GBI Page, nulled out to improve start up time
        /// </summary>
        public GBIControls GBICmd;

        /// <summary>
        /// The Bootloader Page, nulled out to improve startup time;
        /// </summary>
        private BootLoader BtLdr;

        #endregion

        /// <summary>
        /// Tells you whether CAN is initiated or not
        /// Allows Read to end Gracefully
        /// </summary>
        private Boolean isRunning = false;

        /// <summary>
        /// The FilePath, includes select DateTime values for ease of storage
        /// </summary>
        private String FilePath = String.Format(@"C:\\Users\\jv0116\\Desktop\\Logs\\Log {0}_{1}_{2} {3}_{4:00}_{5:00}.txt",
            DateTime.Now.Year, 
            DateTime.Now.Month, 
            DateTime.Now.Day, 
            DateTime.Now.TimeOfDay.Hours, 
            DateTime.Now.TimeOfDay.Minutes, 
            DateTime.Now.Second);

        /// <summary>
        /// the streamwriter to write to
        /// </summary>
        private System.IO.StreamWriter file = null;

        /// <summary>
        /// queue that allows us to queue up read values for use in another thread
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentQueue<MsgTime> que =
            new System.Collections.Concurrent.ConcurrentQueue<MsgTime>();

        /// <summary>
        /// The List of Console Messages to be Posted
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentQueue<System.Text.StringBuilder> MsgLst =
            new System.Collections.Concurrent.ConcurrentQueue<System.Text.StringBuilder>();

        /// <summary>
        /// Used to split processing values and updating the UI
        /// </summary>
        Thread workerThread;

        #region PEAK

        /// <summary>
		/// Save the current initiated hardware type
		/// </summary>
		private HardwareType ActiveHardware;
        
		/// <summary>
		/// CAN messages Array. Store the Message Status for its display
		/// </summary>
		private ArrayList LastMsgsList;

        /// <summary>
	    /// Read Delegate Handler
	    /// </summary>
	    private delegate void ReadDelegateHandler();

	    /// <summary>
	    /// Read Delegate in order to call "ReadMessage" function
	    /// using .NET invoke function
	    /// </summary>
	    private ReadDelegateHandler ReadDelegate;

	    /// <summary>
	    /// Receive-Event
	    /// </summary>
	    private AutoResetEvent RcvEvent;

	    /// <summary>
	    /// Thread in order to read messages using Received-Event method
	    /// </summary>
	    private Thread ReadThread;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Form Start
        /// </summary>
		public ConsoleForm()
		{
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            // We set the variable to know which hardware is 
            // currently selected (none!)
            //
            ActiveHardware = (HardwareType)(-1);
            // Create a list to store the displayed mesasges 
            //
            LastMsgsList = new ArrayList();

            M2R = new MsgToResMethods(this);
            
            ///GBI Page
            GBICmd = null;

            ///Bootloader Page
            BtLdr = null;
		}

        #endregion

        #region Help Functions

        #region Update Message

        #region HEX ID to Msg

        #region Dictionary
        /// <summary>
        /// Dictionary that converts PID to a 3-Letter, User-Friendly Message
        /// </summary>
        System.Collections.Generic.Dictionary<uint, String> PID2MsgDict = new System.Collections.Generic.Dictionary<uint, String>() 
        {
            {0,"NLL"},
            {1,"HI2"},
            {2,"HI1"},
            {3,"CMD"},
            {4,"LW1"},
            {5,"OTR"},
            {6,"DBG"},
            {7,"LW2"}
        };
        #endregion

        /// <summary>
        /// Uses the Dictionary PID2MsgDict to convert an inputed ID to a User Friendly Message
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private String ToPIDMsg(UInt32 ID)
        {
            UInt32 PID = (ID & (int)MSK.CAN_PRIORITY_MASK) >> 26;

            if (PID2MsgDict.ContainsKey(PID))
            {
                return PID2MsgDict[PID];
            }
            else
            {
                return "???";
            }
        }

        /// <summary>
        /// Converts the ID to a more User Friendly Message
        /// </summary>
        /// <param name="TID"></param>
        /// <returns></returns>
        private String ToTIDMsg(UInt32 ID)
        {
            UInt32 TID = ID & (int)MSK.CAN_TYPE_MASK;
            String msg = Enum.Format(typeof(Hologic.CANRouter.TID), (Hologic.CANRouter.TID)TID, "g").Substring(8);

            #region Length Check
            if (msg.Length > MaxTIDLen)
            {
                MaxTIDLen = msg.Length;
            }
            #endregion

            return msg;
        }

        /// <summary>
        /// Converts the GID to a more User Friendly Message
        /// </summary>
        /// <param name="GID"></param>
        /// <returns></returns>
        private String ToGIDMsg(UInt32 ID)
        {
            UInt32 GID = ID & (int)MSK.CAN_GROUP_MASK;
            String msg = Enum.Format(typeof(Hologic.CANRouter.GRP), (Hologic.CANRouter.GRP)GID, "g").Substring(8);

            #region Length Check
            if (msg.Length > MaxGIDLen)
            {
                MaxGIDLen = msg.Length;
            }
            #endregion

            return msg;
        }

        /// <summary>
        /// Converts the MID to a more User Friendly Message
        /// </summary>
        /// <param name="MID"></param>
        /// <returns></returns>
        private String ToMIDMsg(uint ID)
        {
            #region Declarations
            String MID = "";
            uint mid = (ID & (uint)MSK.CAN_MSG_MASK) >> 10;
            uint key = ID & (uint)MSK.CAN_GROUP_MASK;
            #endregion

            #region MID Find
            if (MsgsDividedByGroup.ContainsKey(key))
            {
                MID = MsgsDividedByGroup[key].Find((x) => x.MID == mid).Message.ToString();
            }
            else
            {
                MID = "Error: Invalid Group";
            }
            #endregion

            #region Length Check
            if (MID.Length > MaxMIDLen)
            {
                MaxMIDLen = MID.Length;
            }
            #endregion

            return MID;
        }

        /// <summary>
        /// Converts the DID to a more User Friendly Message
        /// </summary>
        /// <param name="DID"></param>
        /// <returns></returns>
        private String ToDIDMsg(UInt32 ID)
        {
            UInt32 DID = ID & (int)MSK.CAN_DEST_ID_MASK;
            String msg = Enum.Format(typeof(Hologic.CANRouter.DID), (Hologic.CANRouter.DID)DID, "g").Substring(8);

            #region Length Check
            if (msg.Length> MaxDIDLen)
            {
                MaxDIDLen = msg.Length;
            }
            #endregion

            return msg;
        }

        /// <summary>
        /// Converts the SID to a more User Friendly Message
        /// </summary>
        /// <param name="SID"></param>
        /// <returns></returns>
        private String ToSIDMsg(UInt32 ID)
        {
            UInt32 SID = ID & (int)MSK.CAN_SOURCE_ID_MASK;
            String msg = Enum.Format(typeof(Hologic.CANRouter.SID), (Hologic.CANRouter.SID)SID, "g").Substring(8);

            #region Length Check
            if (msg.Length > MaxSIDLen)
            {
                MaxSIDLen = msg.Length;
            }
            #endregion

            return msg;
        }

        #endregion

        #region Messages

        /// <summary>
        /// Public Version of AddConsoleMsg()
        /// Adds the Console Message to the List, to be pulled by DumpConsoleMsg()
        /// </summary>
        /// <param name="q"></param>
        /// <param name="w"></param>
        public void AddOutsideMessage(TCLightMsg q, TCLightTimestamp w)
        {
            AddConsoleMsg(q, w);
        }

        /// <summary>
        /// Adds the Console Message to the List, to be pulled by DumpConsoleMsg()
        /// </summary>
        /// <param name="NewMsg"></param>
        /// <param name="MyTimeStamp"></param>
        private void AddConsoleMsg(TCLightMsg NewMsg, TCLightTimestamp MyTimeStamp)
        {
            #region Message Check

            if (NewMsg == null || MyTimeStamp == null)//if message incomplete
            {
                return;
            }

            #endregion

            #region Data
            UpdateNodes(NewMsg.Data, NewMsg.ID);
            #endregion

            #region Response Handling
            ResponseHandler(NewMsg);
            #endregion

            #region IO
            if (file == null)
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    + "\\Logs");
                FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                FilePath += String.Format("\\Logs\\Log {0:0000}_{1:00}_{2:00} {3:00}_{4:00} {5:00}.txt",
                                          DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                                          DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, 
                                          DateTime.Now.Second);

                file = new System.IO.StreamWriter(FilePath, true);
            }
            #endregion

            #region Checkbox Check
            bool Message = true;
            switch (ToSIDMsg(NewMsg.ID))
            {
                case "AWS":
                    if (!AWSCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "DET":
                    if (!DETCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "DTC":
                    if (!DTCCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "GCB":
                    if (!GCBCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "PMC":
                    if (!PMCCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "GEN":
                    if (!GENCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "VTA":
                    if (!VTACb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "CRM":
                    if (!CRMCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "THD":
                    if (!THDCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "CDI":
                    if (!CDICb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "AIO":
                    if (!AIOCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "BKY":
                    if (!BKYCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "BGM":
                    if (!BGMCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "BCM":
                    if (!BCMCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "TCB":
                    if (!TCBCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                case "ACC":
                    if (!ACCCb.Checked)
                    {
                        Message = false;
                    }
                    break;
                default:
                    if (!OTHCb.Checked)
                    {
                        Message = false;
                    }
                    break;
            }
            #endregion

            #region Message

            ///Should clean this up a bit,
            ///Store message, so it can be reused
            ///Add timestamp if TimeStamp is enabled
            ///consider passing message into new helper method
            
            //MutExStrLst.WaitOne(-1);
            if (HrtEnabled.Checked == true)
            {
                #region Heartbeats

                if (chbTimeStamp.Checked)
                {
                    if (Message)
                    {
                        MsgLst.Enqueue(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                            " " + MyTimeStamp.millis.ToString()));
                    }
                    file.WriteLine(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                        " " + MyTimeStamp.millis.ToString()));
                }
                else
                {
                    if (Message) 
                        MsgLst.Enqueue(BuildID(NewMsg.ID).Append(BuildData(NewMsg)));//.Append(
                             //" " + MyTimeStamp.millis.ToString()));
                    file.WriteLine(BuildID(NewMsg.ID).Append(BuildData(NewMsg)));//.Append(
                        //" " + MyTimeStamp.millis.ToString()));
                }

                #endregion
            }
            else if (ToMIDMsg(NewMsg.ID) != ToMIDMsg((UInt32)MSG.CAN_HEARTBEAT))
            {
                #region No Heartbeats

                if (chbTimeStamp.Checked)
                {
                    if(Message)
                        MsgLst.Enqueue(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                            " " + MyTimeStamp.millis.ToString()));
                    file.WriteLine(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                        " " + MyTimeStamp.millis.ToString()));
                }
                else
                {
                    if (Message)
                        MsgLst.Enqueue(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                            " " + MyTimeStamp.millis.ToString()));
                    file.WriteLine(BuildID(NewMsg.ID).Append(BuildData(NewMsg)).Append(
                        " " + MyTimeStamp.millis.ToString()));
                }

                #endregion
            }
            //MutExStrLst.ReleaseMutex();

            #endregion

            #region IO
            try
            {
                file.Flush();
            } catch {
                return;
            }
            if (new System.IO.FileInfo(FilePath).Length >= 1048576 * 10)//approx 10 Mb max
            {
                //prepares the file to start over annew
                file = null;
            }
            #endregion
        }

        /// <summary>
        /// Adds the Console Message to the List, to be pulled by DumpConsoleMsg()
        /// </summary>
        /// <param name="NewMsg"></param>
        public void AddMiscMsg(String msg)
        {
            MsgLst.Enqueue(new StringBuilder(msg));
        }

        /// <summary>
        /// empties the list onto the Console Message ListBox
        /// </summary>
        private void DumpConsoleMsg()
        {
            if (MsgLst.Count > 0)
            {
                PostStatusMessages(MsgLst);
            }
        }

        #endregion

        #region Build Message

        /// <summary>
        /// Takes an ID and Returns Messages
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private System.Text.StringBuilder BuildID(UInt32 ID)
        {
            int Length = 56 + MaxTIDLen + MaxGIDLen + MaxMIDLen + MaxDIDLen + MaxSIDLen;
            System.Text.StringBuilder Out = new System.Text.StringBuilder("",Length);

            String msgPiece = "";

            #region ID
            if (IDCheck.Checked)
            {
                Out.AppendFormat("{0,-10:X8}", ID);
            }
            else
            {
                Out.AppendFormat("{0,-10}","");
            }
            #endregion

            #region Priority ID
            if (PIDChecked.Checked)
            {
                Out.AppendFormat("{0,-5}", ToPIDMsg(ID));
            }
            else
            {
                Out.AppendFormat("{0,-5}","");
            }
            #endregion

            #region Type ID
            if (TIDChecked.Checked)
            {
                msgPiece = ToTIDMsg(ID);
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxTIDLen) + "}  ", msgPiece);
            }
            else
            {
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxTIDLen) + "}  ", "");
            }
            #endregion

            #region Group ID
            if (GIDChecked.Checked)
            {
                msgPiece = ToGIDMsg(ID);
                Out.AppendFormat("{0,-"  + String.Format("{0}", MaxGIDLen) + "}   ", msgPiece);
            }
            else
            {
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxGIDLen) + "}   ", "");
            }
            #endregion

            #region Message ID

            if (MIDChecked.Checked)
            {
                msgPiece = ToMIDMsg(ID);
                Out.AppendFormat("{0,-" +  String.Format("{0}", MaxMIDLen) + "}   ", msgPiece);
            }
            else
            {
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxMIDLen) + "}   ", "");
            }

            #endregion

            #region Source ID
            if (SIDChecked.Checked)
            {
                msgPiece = ToSIDMsg(ID);
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxSIDLen) + "}-->", msgPiece);
            }
            else
            {
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxSIDLen) + "}-->", "");
            }
            #endregion

            #region Destination ID
            if (DIDChecked.Checked)
            {

                msgPiece = ToDIDMsg(ID);
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxDIDLen) + "}  ", msgPiece);
            }
            else
            {
                Out.AppendFormat("{0,-" + String.Format("{0}", MaxDIDLen) + "}  ", "");
            }
            #endregion

            return Out;
        }

        /// <summary>
        /// Outputs the Data in 00 00 00 00 00 00 00 00 format
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private System.Text.StringBuilder BuildData(TCLightMsg msg)
        {
            if (DataChecked.Checked)
            {
                System.Text.StringBuilder DataStr = new System.Text.StringBuilder("", 30);
                int i = 1;
                foreach (Byte a in msg.Data)
                {
                    if (i < msg.Len)
                    {
                        //notice space after the '}'
                        DataStr.AppendFormat("{0:X2} ", a);
                    }
                    else
                    {
                        DataStr.Append("   ");
                    }
                }
                return DataStr;
            }
            else
            {
                System.Text.StringBuilder DataStr = new System.Text.StringBuilder(30);
                DataStr.AppendFormat("{0,24}", "");
                return DataStr;
            }
        }

        /// <summary>
        /// Creates a local Dictionary of Lists of Messages, organized by Group
        /// </summary>
        /// <returns>A Dictionary of Lists of Messages</returns>
        private Dictionary<uint, List<MSGListVal>> GetMessagesDividedByGroup()
        {
            Dictionary<uint, List<MSGListVal>> dict = new Dictionary<uint, List<MSGListVal>>();
            uint group;
            foreach (MSG m in Enum.GetValues(typeof(MSG)))
            {
                group = (uint)MSK.CAN_GROUP_MASK & (uint)m;

                if (!dict.ContainsKey(group))
                {
                    dict.Add(group, new List<MSGListVal>());
                }

                MSGListVal q = new MSGListVal(m, (((uint)m & (uint)MSK.CAN_MSG_MASK)>>10));
                dict[group].Add(q);
            }

            //#region IO
            ///*
            //FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //FilePath += String.Format("\\Logs\\MessageToMIDVals.txt",
            //    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, DateTime.Now.Second);

            //System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            //    + "\\Logs");
            //file = new System.IO.StreamWriter(FilePath, false);
            //*/
            //#endregion

            //foreach (List<MSGListVal> q in dict.Values)
            //{
            //    uint Count = 0;
            //    uint MaxVal = 0;

            //    file.WriteLine("_________________________" + 
            //                  (GRP)((uint)q[0].Message & (uint)MSK.CAN_GROUP_MASK) + 
            //                   "_________________________");
            //    foreach (MSGListVal w in q)
            //    {
            //        file.WriteLine(w.Message.ToString().PadRight(35) + w.MID);
            //        Count++;
            //        if (MaxVal < w.MID)
            //        {
            //            MaxVal = w.MID;
            //        }
            //    }

            //    if (Count - 1 == MaxVal)
            //    {
            //        file.WriteLine("Count vs Max:".PadRight(33) + (Count - 1) + " = " + MaxVal);
            //    }
            //    else
            //    {
            //        file.WriteLine("Count vs Max:".PadRight(33) + (Count - 1) + " != " + MaxVal);
            //    }
            //}
            //file.Flush();

            //file = null;
            //Returns a dictionary, separated by grp ID, where each element is the list of messages.
            return dict;
        }

        #endregion

        #region Post Status
        /// <summary>
        /// Updates the List Box with new Messages
        /// </summary>
        /// <param name="msg"></param>
        public void PostStatusMessages(List<System.Text.StringBuilder> msgs)
        {
            if (ConsoleListBox.InvokeRequired)
            {
                ConsoleListBox.BeginInvoke(new Action(delegate
                {
                    PostStatusMessages(msgs);
                }));
            }
            else
            {
                if (msgs == null || msgs.Count == 0)
                    return;

                for (int i = 0;
                    i < msgs.Count; //(numericUpDown1.Value/tmrDump.Interval < msgs.Count - 1? (numericUpDown1.Value/tmrDump.Interval) : (msgs.Count - 1));
                    i++)
                {
                    ConsoleListBox.BeginUpdate();
                    if (ConsoleListBox.Items.Count > 2000)
                    {
                        ConsoleListBox.Items.RemoveAt(2000);
                    }
                    ConsoleListBox.Items.Insert(0, msgs[i]);
                    ConsoleListBox.EndUpdate();
                }
            }
        }
        /// <summary>
        /// Updates the List Box with new Messages
        /// </summary>
        /// <param name="msgs"></param>
        public void PostStatusMessages(ConcurrentQueue<System.Text.StringBuilder> msgs)
        {
            if (ConsoleListBox.InvokeRequired)
            {
                ConsoleListBox.BeginInvoke(new Action(delegate
                {
                    PostStatusMessages(msgs);
                }));
            }
            else
            {
                if (msgs == null || msgs.Count < 0)
                    return;


                StringBuilder msg;

                for (int i = 0;
                    i < msgs.Count;//(numericUpDown1.Value / tmrDump.Interval < msgs.Count - 1 ? (numericUpDown1.Value / tmrDump.Interval) : (msgs.Count - 1));
                    i++)
                {
                    ConsoleListBox.BeginUpdate();
                    if (ConsoleListBox.Items.Count > 2000)//max size = 2000
                    {
                        ConsoleListBox.Items.RemoveAt(2000);
                    }
                    msgs.TryDequeue(out msg);
                    ConsoleListBox.Items.Insert(0, msg);
                    ConsoleListBox.EndUpdate();
                }
            }
        }
        /// <summary>
        /// Updates the List Box with the new Message
        /// </summary>
        /// <param name="msg"></param>
        public void PostStatusMessage(System.Text.StringBuilder msg)
        {
            if (ConsoleListBox.InvokeRequired)
            {
                ConsoleListBox.BeginInvoke(new Action(delegate
                {
                    PostStatusMessage(msg);
                }));
            }
            else
            {
                if (msg == null)
                    return;

                //for (int i = 0;
                //    i < (numericUpDown1.Value / tmrDump.Interval < msgs.Count - 1 ? (numericUpDown1.Value / tmrDump.Interval) : (msgs.Count - 1));
                //    i++)
                //{
                    //ConsoleListBox.BeginUpdate();
                    if (ConsoleListBox.Items.Count > 2000)
                    {
                        ConsoleListBox.Items.RemoveAt(2000);
                    }
                    ConsoleListBox.Items.Insert(0, msg);
                    //ConsoleListBox.EndUpdate();
                //}
            }
        }
        #endregion

        #endregion

        #region Update Nodes

        /// <summary>
        /// Gets the appropriate text box
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="LastNode">passes the reference to the previous Text Box back to the caller function</param>
        /// <returns></returns>
        private TextBox[] GetTextBoxes( UInt32 ID, out UInt32 LastNode )
        {
            switch (ToSIDMsg(ID))
            {
                case "AWS":
                    TextBox[] aws = new TextBox[3];
                    aws[0] = IdTbAws;
                    aws[1] = StatusTbAws;
                    aws[2] = FaultTbAws;
                    LastNode = LastAWS;
                    LastAWS = ID;
                    return aws;
                case "DET":
                    TextBox[] det = new TextBox[3];
                    det[0] = IdTbDet;
                    det[1] = StatusTbDet;
                    det[2] = FaultTbDet;
                    LastNode = LastDET;
                    LastDET = ID;
                    return det;
                case "DTC":
                    TextBox[] Dtc = new TextBox[3];
                    Dtc[0] = IdTbDtc;
                    Dtc[1] = StatusTbDtc;
                    Dtc[2] = FaultTbDtc;
                    LastNode = LastDTC;
                    LastDTC = ID;
                    return Dtc;
                case "GCB":
                    TextBox[] Gcb = new TextBox[3];
                    Gcb[0] = IdTbGcb;
                    Gcb[1] = StatusTbGcb;
                    Gcb[2] = FaultTbGcb;
                    LastNode = LastGCB;
                    LastGCB = ID;
                    return Gcb;
                case "PMC":
                    TextBox[] Pmc = new TextBox[3];
                    Pmc[0] = IdTbPmc;
                    Pmc[1] = StatusTbPmc;
                    Pmc[2] = FaultTbPmc;
                    LastNode = LastPMC;
                    LastPMC = ID;
                    return Pmc;
                case "GEN":
                    TextBox[] Gen = new TextBox[3];
                    Gen[0] = IdTbGen;
                    Gen[1] = StatusTbGen;
                    Gen[2] = FaultTbGen;
                    LastNode = LastGEN;
                    LastGEN = ID;
                    return Gen;
                case "VTA":
                    TextBox[] Vta = new TextBox[3];
                    Vta[0] = IdTbVta;
                    Vta[1] = StatusTbVta;
                    Vta[2] = FaultTbVta;
                    LastNode = LastVTA;
                    LastVTA = ID;
                    return Vta;
                case "CARM":
                    TextBox[] Crm = new TextBox[3];
                    Crm[0] = IdTbCrm;
                    Crm[1] = StatusTbCrm;
                    Crm[2] = FaultTbCrm;
                    LastNode = LastCRM;
                    LastCRM = ID;
                    return Crm;
                case "THD":
                    TextBox[] Thd = new TextBox[3];
                    Thd[0] = IdTbThd;
                    Thd[1] = StatusTbThd;
                    Thd[2] = FaultTbThd;
                    LastNode = LastTHD;
                    LastTHD = ID;
                    return Thd;
                case "CDI":
                    TextBox[] Cdi = new TextBox[3];
                    Cdi[0] = IdTbCdi;
                    Cdi[1] = StatusTbCdi;
                    Cdi[2] = FaultTbCdi;
                    LastNode = LastCDI;
                    LastCDI = ID;
                    return Cdi;
                case "AIB":
                    TextBox[] Aio = new TextBox[3];
                    Aio[0] = IdTbAio;
                    Aio[1] = StatusTbAio;
                    Aio[2] = FaultTbAio;
                    LastNode = LastAIO;
                    LastAIO = ID;
                    return Aio;
                case "BKY":
                    TextBox[] Bky = new TextBox[3];
                    Bky[0] = IdTbBky;
                    Bky[1] = StatusTbBky;
                    Bky[2] = FaultTbBky;
                    LastNode = LastBKY;
                    LastBKY = ID;
                    return Bky;
                case "BGM":
                    TextBox[] Bgm = new TextBox[3];
                    Bgm[0] = IdTbBgm;
                    Bgm[1] = StatusTbBgm;
                    Bgm[2] = FaultTbBgm;
                    LastNode = LastBGM;
                    LastBGM = ID;
                    return Bgm;
                case "BCM":
                    TextBox[] Bcm = new TextBox[3];
                    Bcm[0] = IdTbBcm;
                    Bcm[1] = StatusTbBcm;
                    Bcm[2] = FaultTbBcm;
                    LastNode = LastBCM;
                    LastBCM = ID;
                    return Bcm;
                case "TCB":
                    TextBox[] Tcb = new TextBox[3];
                    Tcb[0] = IdTbTcb;
                    Tcb[1] = StatusTbTcb;
                    Tcb[2] = FaultTbTcb;
                    LastNode = LastTCB;
                    LastTCB = ID;
                    return Tcb;
                case "ACC":
                    TextBox[] Acc = new TextBox[3];
                    Acc[0] = IdTbAcc;
                    Acc[1] = StatusTbAcc;
                    Acc[2] = FaultTbAcc;
                    LastNode = LastACC;
                    LastACC = ID;
                    return Acc;
                default:
                    LastNode = 0;
                    return null;
            }
        }

        /// <summary>
        /// Updates the Appropriate Text Boxes with the Appropriate values
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="ID"></param>
        private void UpdateNodes(Byte[] Data, UInt32 ID)
        {
            #region Invoke
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    UpdateNodes(Data, ID);
                }));
                return;
            }
            #endregion

            #region Get TB
            UInt32 LastNode;

            TextBox[] textBox = GetTextBoxes(ID, out LastNode);
            #endregion

            #region ValCheck

            //if node value did NOT change, then escape
            if (LastNode == ID)
            {
                return;
            }
            //else, continue

            if (textBox == null)
            {
                //unknown();
                return;
            }

            #endregion

            #region State Information
            switch (Data[1])
            {
                case 0:
                    //normal
                    textBox[2].Text = "Normal";
                    break;
                case 1:
                    //NPT
                    textBox[2].Text = "NPT";
                    break;
                case 2:
                    //Calibration
                    textBox[2].Text = "Calibration";
                    break;
                default:
                    //Unknown
                    //textBox[2].Text = "UNK";//doesn't work, needs new textbox for unknown vals
                    break;
            }
            #endregion

            #region Status
            switch (Data[0])
            {
                case 0:
                    ready(Data, textBox[1], textBox[0]);
                    break;
                case 1:
                    inProgress(Data, textBox[1], textBox[0]);
                    break;
                case 2:
                    standby(Data, textBox[1], textBox[0]);
                    break;
                case 4:
                    config(Data, textBox[1], textBox[0]);
                    break;
                case 8:
                    sync(Data, textBox[1], textBox[0]);
                    break;
                default:
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Updates the Appropriate Node with the ready status
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="tb1"></param>
        /// <param name="tb2"></param>
        private void ready(Byte[] Data, TextBox tb1, TextBox tb2)
        {
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    ready(Data, tb1, tb2);
                }));
                return;
            }
            tb1.Text = "Ready";
            if (Data[2] == 0)
            {
                tb2.Text = "Normal";
            }
            else if (Data[2] == 1)
            {
                tb2.Text = "Notification";
            }
            else
            {
                tb2.Text = "Unknown";
            }
        }

        /// <summary>
        /// Updates the Appropriate Text Box with the InProg Status
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="tb1"></param>
        /// <param name="tb2"></param>
        private void inProgress(Byte[] Data, TextBox tb1, TextBox tb2)
        {
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    inProgress(Data, tb1, tb2);
                }));
                return;
            }
            tb1.Text = "InProg";
            if (Data[2] == 0)
            {
                tb2.Text = "Normal";
            }
            else if (Data[2] == 1)
            {
                tb2.Text = "Notification";
            }
            else
            {
                tb2.Text = "Unknown";
            }
        }

        /// <summary>
        /// Updates the Appropriate Text Box with the Standby status
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="tb1"></param>
        /// <param name="tb2"></param>
        private void standby(Byte[] Data, TextBox tb1, TextBox tb2)
        {
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    standby(Data, tb1, tb2);
                }));
                return;
            }
            tb1.Text = "Standby";
            switch (Data[2])
            {
                case 0:
                    tb2.Text = "Idle";
                    break;
                case 1:
                    tb2.Text = "Notification";
                    break;
                case 2:
                    tb2.Text = "Busy";
                    break;
                case 4:
                    tb2.Text = "Start/Stop";
                    break;
                case 8:
                    tb2.Text = "Data Req'd";
                    break;
                default:
                    tb2.Text = "Unknown";
                    break;
            }
        }

        /// <summary>
        /// Updates the Appropriate Text Box with the Config Status
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="tb1"></param>
        /// <param name="tb2"></param>
        private void config(Byte[] Data, TextBox tb1, TextBox tb2)
        {
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    config(Data, tb1, tb2);
                }));
                return;
            }
            tb1.Text = "Config";
            switch (Data[2])
            {
                case 0:
                    tb2.Text = "Idle";
                    break;
                case 1:
                    tb2.Text = "Notification";
                    break;
                case 2:
                    tb2.Text = "UnInit";
                    break;
                case 4:
                    tb2.Text = "Rdy4Download";
                    break;
                case 8:
                    tb2.Text = "UnCalibrated";
                    break;
                default:
                    tb2.Text = "Unknown";
                    break;
            }
        }

        /// <summary>
        /// Updates the Appropriate Text Box with the Sync Status
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="tb1"></param>
        /// <param name="tb2"></param>
        private void sync(Byte[] Data, TextBox tb1, TextBox tb2)
        {
            if (panel3.InvokeRequired)
            {
                panel3.BeginInvoke(new Action(delegate
                {
                    sync(Data, tb1, tb2);
                }));
                return;
            }
            tb1.Text = "Sync";
            switch (Data[2])
            {
                case 0:
                    tb2.Text = "Idle";
                    break;
                case 1:
                    tb2.Text = "Notification";
                    break;
                case 2:
                    tb2.Text = "UnInit";
                    break;
                case 4:
                    tb2.Text = "Rdy4Download";
                    break;
                case 8:
                    tb2.Text = "UnCalibrated";
                    break;
                default:
                    tb2.Text = "Unknown";
                    break;
            }
        }
        
        #endregion

        #region Message Response

        public void AddMsgToResponse(uint msg, Action<TCLightMsg> method)
        {
            try
            {
                MsgToResponse.Add(msg, method);
            }
            catch { }
        }

        private void ResponseHandler(TCLightMsg NewMsg)
        {
            //this bit finds a message as stated in the Dictionary
            //(uint)MsgsDividedByGroup[key].Find((x) => x.MID == mid).Message;
            try
            {
                if (MsgToResponse.Count > 0)
                {
                    uint mid = (NewMsg.ID & (uint)MSK.CAN_MSG_MASK) >> 10;
                    uint key = NewMsg.ID & (uint)MSK.CAN_GROUP_MASK;

                    MsgToResponse[(uint)MsgsDividedByGroup[key].Find((x) => x.MID == mid).Message](NewMsg);
                }
            }
            catch (KeyNotFoundException) { return; }
        }

        public static void RemoveMsgToResponse(uint msg)
        {
            MsgToResponse.Remove(msg);
        }

        public bool MsgEqual(uint baseMsg, uint compareMsg)
        {
            uint baseMid = (baseMsg & (uint)MSK.CAN_MSG_MASK) >> 10;
            uint baseKey = baseMsg & (uint)MSK.CAN_GROUP_MASK;

            uint compareMid = (compareMsg & (uint)MSK.CAN_MSG_MASK) >> 10;
            uint compareKey = compareMsg & (uint)MSK.CAN_GROUP_MASK;

            if (MsgsDividedByGroup[baseKey].Find((x) => x.MID == baseMid).Message == MsgsDividedByGroup[compareKey].Find((x) => x.MID == compareMid).Message)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Misc.
        // This helper function retrieve active Dll Major version number
	    private CANResult DllMajorVersion(HardwareType hType, out int majorVersion)
	    {
            CANResult Res;
		    String dllVersionStr = "";

		    // We execute the "DllVersionInfo" function of the PCANLight
		    // using as parameter the Hardware type and a string
		    // variable to get the info like "x.xx"
		    //
            Res = PCANLight.DllVersionInfo(hType, out dllVersionStr);

		    // We retrieve major version number 
		    // spliting versionNumberStr based on "." decimal symbol
		    //
            String[] versionTabInfo = dllVersionStr.Split('.');
            if (versionTabInfo.Length > 0)
                Int32.TryParse(versionTabInfo[0].ToString(), out majorVersion);
            else
                majorVersion = 0;
		    return Res;
	    }

        private void CANReadThreadFunc() 
	    {
		    // Sets the handle of the Receive-Event.
		    //
            PCANLight.SetRcvEvent(ActiveHardware, RcvEvent);

		    // While this mode is selected
		    while(rdbEvent.Checked)
		    {
			    // Waiting for Receive-Event
			    // 
			    RcvEvent.WaitOne();

			    // Process Receive-Event using .NET Invoke function
			    // in order to interact with Winforms UI
			    // 
			    this.Invoke(ReadDelegate);
		    }
	    }

        #endregion

        #region GBI Page

        /// <summary>
        /// Deletes and Re-Creates the GBI Page, thus Reseting the values
        /// </summary>
        public void ResetGBIPage()
        {
            GBICmd.Close();
            GBICmd = new GBIControls(this);
            GBICmd.Show();
        }

        #endregion

        #region Read

        /// <summary>
        /// Does Basic Hardware/Software Checking and adds msg to queue
        /// </summary>
        private void ReadMessage() 
	    {
		    TCLightMsg MyMsg;
            TCLightTimestamp MyTimeStamp = null;
		    CANResult Res;
    	
		    // We read at least one time the queue looking for messages.
		    // If a message is found, we look again trying to find more.
		    // If the queue is empty or an error occurr, we get out from
		    // the dowhile statement.
		    //			
		    do
		    {
			    // We read the queue looking for messages.
			    //
			    //if(chbTimeStamp.Checked)
				    // We execute the "ReadEx" function of the PCANLight
				    // if "Show Time Stamp" checkbox is selected
				    //
                    Res = PCANLight.ReadEx(ActiveHardware, out MyMsg, out MyTimeStamp);
			    //else
				    // We execute the "Read" function of the PCANLight
				    // if "Show Time Stamp" checkbox isn't selected
				    //
                    //Res = PCANLight.Read(ActiveHardware, out MyMsg);

			    // A message was received
			    // We process the message(s)
			    //
                    if (Res == CANResult.ERR_OK)
                    {
                        //MutExMsgQue.WaitOne(-1);
                        que.Enqueue(new MsgTime(MyMsg, MyTimeStamp));
                        //MutExMsgQue.ReleaseMutex();
                        //AddConsoleMsg(MyMsg, MyTimeStamp);
                    }

            } while (((int)ActiveHardware != -1) && (!Convert.ToBoolean(Res & CANResult.ERR_QRCVEMPTY)));//loops until fails
	    }

        /// <summary>
        /// Dequeues and updates the message
        /// </summary>
        void AddConsoleMsgs()
        {
            while (true)
            {
                if (que.Count > 0)
                {
                    //MutExMsgQue.WaitOne(-1);
                    MsgTime q;
                    que.TryDequeue(out q);
                    //MutExMsgQue.ReleaseMutex();
                    AddConsoleMsg(q.CANMessage, q.Timestamp);
                }
                else if (!isRunning)//finishes, then escapes
                {
                    break;
                }
            }
        }

        #endregion

        #region Getter/Setter(s)

        public HardwareType GetActiveHardware()
        {
            return ActiveHardware;
        }

        #endregion

        #endregion

        #region Form Functions

        #region Open Page

        private void Form1_Load(object sender, System.EventArgs e)
		{
			// Set the standard values in the interface
			//
			cbbHws.SelectedIndex = 6;
			cbbBaudrates.SelectedIndex = 1;
			cbbIO.Text = "0378";
			cbbInterrupt.Text = "7";
			cbbMsgType.SelectedIndex = 1;
            chbTimeStamp.Enabled = true;
			
            // Create Delegates to use invoke() function
			//
			ReadDelegate = new ReadDelegateHandler(this.ReadMessage);
			// Create AutoResetEvent to use PCLight SetRcvEvent() function
			//
			RcvEvent = new AutoResetEvent(false);
		}

        private void GBIFormBtn_Click(object sender, EventArgs e)
        {
            //if not created yet, create
            if (GBICmd == null)
            {
                GBICmd = new GBIControls(this);
            }

            AddMsgToResponse((uint)MSG.CAN_VAL_NODE_TEST_FUNCTION, M2R.ValNodeTestFunctionResponse);
            GBICmd.RequestSettings();
            
            //if created, swap visibility
            if (!GBICmd.Visible)
            {
                GBICmd.Show();
            }
            else
            {
                GBICmd.Hide();
            }
        }

        private void MMICommand_Click(object sender, EventArgs e)
        {
            MMICommandForm MMICmd = new MMICommandForm(ActiveHardware, this);
            MMICmd.Show();
        }

        private void BootLoaderBtn_Click(object sender, EventArgs e)
        {
            if (BtLdr == null)
            {
                BtLdr = new BootLoader(this);
            }

            if (!BtLdr.Visible)
            {
                BtLdr.Show();
            }
            else
            {
                BtLdr.Hide();
            }
        }

        #endregion

        #region Close Page

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnRelease.Enabled)
                btnRelease_Click(this, new EventArgs());

            // If we are reading, we stop to read
            // comment out tmrDump.Enabled if we get errors
            tmrDump.Enabled = false;
            tmrRead.Enabled = false;
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            // We terminate the application
            //
            Close();
        }

        #endregion

        #region Console

        private void btnInit_Click(object sender, System.EventArgs e)
        {
            isRunning = true;

            workerThread = new Thread(AddConsoleMsgs);
            workerThread.IsBackground = true;
            workerThread.Start();

            if (MsgsDividedByGroup == null)
                MsgsDividedByGroup = GetMessagesDividedByGroup();

            CANResult Res;
            int majorVersion = 0;

            // Check version 2.x Dll is available
            //
            Res = DllMajorVersion((HardwareType)cbbHws.SelectedIndex, out majorVersion);
            if (Res == CANResult.ERR_OK)
            {
                // Sample must ONLY work if a 2.x or later version of the
                // PCAN-Light is installed
                //
                if (majorVersion < 2)
                {
                    MessageBox.Show("DLL 2.x or later are required to run this program" +
                        "\r\nPlease, download lastest DLL version on http://www.peak-system.com or refer to the documentation for more information.",
                        "DLL Version", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // According with the active parameters/hardware, we
                    // use one of the two possible "Init" PCANLight functions.
                    // One is for Plug And Play hardware, and the other for
                    // Not P&P.
                    //
                    if (cbbIO.Enabled)
                        // Not P&P Hardware
                        //
                        Res = PCANLight.Init((HardwareType)cbbHws.SelectedIndex,
                            (Baudrates)cbbBaudrates.Tag,
                            (FramesType)cbbMsgType.SelectedIndex,
                            Convert.ToUInt32(cbbIO.Text, 16),
                            Convert.ToByte(cbbInterrupt.Text));
                    else
                        // P&P Hardware
                        //
                        Res = PCANLight.Init((HardwareType)cbbHws.SelectedIndex,
                            (Baudrates)cbbBaudrates.Tag,
                            (FramesType)cbbMsgType.SelectedIndex);

                    // The Hardware was successfully initiated
                    //
                    if (Res == CANResult.ERR_OK)
                    {
                        // We save the hardware type which is currently 
                        // initiated
                        //
                        ActiveHardware = (HardwareType)cbbHws.SelectedIndex;

                        // We start to read from the CAN Queue
                        //
                        tmrDump.Enabled = true;
                        tmrRead.Enabled = true;

                        // Set UI enable
                        btnInit.Enabled = !(btnWrite.Enabled = btnSetFilter.Enabled = btnResetFilter.Enabled = btnRelease.Enabled = btnInfo.Enabled = btnDllInfo.Enabled = true);
                        rdbTimer.Enabled = true;
                        rdbEvent.Enabled = true;
                        chbTimeStamp.Enabled = true;
                        cbbHws_SelectedIndexChanged(this, new EventArgs());

                        // We show the information of the configured 
                        // and initiated hardware
                        //
                        txtInfo.Text = "Active Hardware: " + cbbHws.Text;
                        txtInfo.Text += "\r\nBaud Rate: " + cbbBaudrates.Text;
                        txtInfo.Text += "\r\nFrame Type: " + cbbMsgType.Text;
                        // If was a no P&P Hardware, we show additional information
                        //
                        if (cbbIO.Enabled)
                        {
                            txtInfo.Text += "\r\nI/O Addr.: " + cbbIO.Text + "h";
                            txtInfo.Text += "\r\nInterrupt: " + cbbInterrupt.Text;
                        }
                    }
                    // An error occurred.  We show the error.
                    //
                    else
                        txtInfo.Text = "Error: " + Res.ToString();
                }
            }
            else
                txtInfo.Text = "Error: " + Res.ToString();
        }

        private void btnRelease_Click(object sender, System.EventArgs e)
        {
            CANResult Res;

            // We stopt to read from the CAN Queue
            //
            tmrRead.Enabled = false;
            tmrDump.Enabled = false;
            isRunning = false;

            // We choose Timer method by default
            //
            rdbTimer.Checked = true;

            // We close the active hardware using the 
            // "Close" function of the PCANLight using 
            // as parameter the Hardware type.
            //
            Res = PCANLight.Close(ActiveHardware);

            // The Hardware was successfully closed
            //
            if (Res == CANResult.ERR_OK)
                txtInfo.Text = "Hardware was successfully Released.\r\n";
            // An error occurred.  We show the error.
            //			
            else
                txtInfo.Text = "Error: " + Res.ToString();

            // We set the varibale of active hardware to None
            // and activate/deactivate the corresponding buttons
            //
            ActiveHardware = (HardwareType)(-1);
            btnInit.Enabled = !(btnWrite.Enabled = btnSetFilter.Enabled = btnResetFilter.Enabled = btnRelease.Enabled = btnInfo.Enabled = btnDllInfo.Enabled = false);
            rdbTimer.Enabled = false;
            rdbEvent.Enabled = false;
            cbbHws_SelectedIndexChanged(this, new EventArgs());
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (ConsoleListBox.InvokeRequired)
            {
                ConsoleListBox.BeginInvoke(new Action(delegate
                {
                    ClearButton_Click(sender, e);
                }));
                return;
            }
            ConsoleListBox.Items.Clear();
        }

        #region Timers

        private void tmrRead_Tick(object sender, System.EventArgs e)
        {
            ReadMessage();
        }

        private void tmrDump_Tick(object sender, EventArgs e)
        {
            DumpConsoleMsg();
        }

        #endregion

        #endregion

        #region Other

        private void ALLCb_CheckedChanged(object sender, EventArgs e)
        {
            AWSCb.Checked = ALLCb.Checked;
            DETCb.Checked = ALLCb.Checked;
            DTCCb.Checked = ALLCb.Checked;
            GCBCb.Checked = ALLCb.Checked;
            PMCCb.Checked = ALLCb.Checked;
            GENCb.Checked = ALLCb.Checked;
            VTACb.Checked = ALLCb.Checked;
            CRMCb.Checked = ALLCb.Checked;
            THDCb.Checked = ALLCb.Checked;
            CDICb.Checked = ALLCb.Checked;
            AIOCb.Checked = ALLCb.Checked;
            BKYCb.Checked = ALLCb.Checked;
            BGMCb.Checked = ALLCb.Checked;
            BCMCb.Checked = ALLCb.Checked;
            TCBCb.Checked = ALLCb.Checked;
            ACCCb.Checked = ALLCb.Checked;
            OTHCb.Checked = ALLCb.Checked;
        }

        #endregion

        #region Legacy

        private void cbbHws_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            bool bShowIO;
            HardwareType current;

            current = (HardwareType)cbbHws.SelectedIndex;

            // According with the selection in the Hardware list, 
            // we Enable/Disable the input controls for I/O Address and 
            // Interrupt. (This parameters are NOT necessary for all 
            // hardware types) .
            //
            switch (current)
            {
                case HardwareType.DNG:
                    bShowIO = true;
                    break;
                case HardwareType.DNP:
                    bShowIO = true;
                    break;
                case HardwareType.ISA_1CH:
                    bShowIO = true;
                    break;
                case HardwareType.ISA_2CH:
                    bShowIO = true;
                    break;
                default:
                    bShowIO = false;
                    break;
            }
            cbbIO.Enabled = bShowIO;
            cbbInterrupt.Enabled = bShowIO;

            // According with the selection in the Hardware list, we 
            // Enable/Disable the controls for Get/Set the USB device Number.
            //
            btnGetUsbDevNumber.Enabled = ((current == HardwareType.USB_1CH) || (current == HardwareType.USB_2CH)) && btnWrite.Enabled;
            btnSetUsbDevNumber.Enabled = btnGetUsbDevNumber.Enabled;
            txtDevNumber.Enabled = btnGetUsbDevNumber.Enabled;
		}

		private void cbbBaudrates_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// We save the corresponding Baudrate enumeration 
			// type value for every selected Baudrate from the 
			// list.
			//
			switch(cbbBaudrates.SelectedIndex)
			{
				case 0:
					cbbBaudrates.Tag = Baudrates.BAUD_1M;
					break;
				case 1:
					cbbBaudrates.Tag = Baudrates.BAUD_500K;
					break;
				case 2:
					cbbBaudrates.Tag = Baudrates.BAUD_250K;
					break;				
				case 3:
					cbbBaudrates.Tag = Baudrates.BAUD_125K;
					break;				
				case 4:
					cbbBaudrates.Tag = Baudrates.BAUD_100K;
					break;				
				case 5:
					cbbBaudrates.Tag = Baudrates.BAUD_50K;
					break;				
				case 6:
					cbbBaudrates.Tag = Baudrates.BAUD_20K;					
					break;				
				case 7:					
					cbbBaudrates.Tag = Baudrates.BAUD_10K;
					break;				
				case 8:
					cbbBaudrates.Tag = Baudrates.BAUD_5K;
					break;		
				default:
					cbbBaudrates.Tag = 0;
					break;
			}		
		}

		private void txtID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			char chCheck;

			// We convert the Character to its Upper case equivalent
			//
			chCheck = char.ToUpper(e.KeyChar);

			// The Key is the Delete (Backspace) Key
			//
			if(chCheck == 8)
				return;
			// The Key is a number between 0-9
			//
			if((chCheck > 47)&&(chCheck < 58))
				return;
			// The Key is a character between A-F
			//
			if((chCheck > 64)&&(chCheck < 71))
				return;

			// Is neither a number nor a character between A(a) and F(f)
			//
			e.Handled = true;			
		}

		private void txtID_Leave(object sender, System.EventArgs e)
		{
			int TextLength;
			uint MaxValue;

			// calculate the text length and Maximum ID value according
			// with the Message Type
			//
			TextLength = (chbExtended.Checked) ? 8 : 3;
			MaxValue = (chbExtended.Checked) ? (uint)0x1FFFFFF : (uint)0x7FF;

			// The Textbox for the ID is represented with 3 characters for 
			// Standard and 8 characters for extended messages.
			// Therefore if the Length of the text is smaller than TextLength,  
			// we add "0"
			//
            while (txtID.Text.Length != TextLength)
                txtID.Text = ("0" + txtID.Text);

			// Because in this example will be sent only Standard messages
			// we check that the ID is not bigger than 0x7FF
			//
            if (Convert.ToUInt32(txtID.Text,16) > MaxValue)
                txtID.Text = string.Format("{0:X" + TextLength.ToString() + "}", MaxValue);				
		}

		private void nudLength_ValueChanged(object sender, System.EventArgs e)
		{
			TextBox CurrentTextBox;

			CurrentTextBox = txtData0;

			// We enable so much TextBox Data fields as the length of the
			// message will be, that is the value of the UpDown control.
			// 
			for(int i=0; i< 8; i++)
			{
				CurrentTextBox.Enabled = (i < nudLength.Value)? true : false;
				if(i < 7)
					CurrentTextBox = (TextBox)this.GetNextControl(CurrentTextBox,true);
			}				
		}

		private void txtData0_Leave(object sender, System.EventArgs e)
		{
			TextBox CurrentTextbox;
			
			// all the Textbox Data fields are represented with 2 characters.
			// Therefore if the Length of the text is smaller than 2, we add
			// a "0"
			//
			if(sender.GetType().Name == "TextBox")
			{				
				CurrentTextbox = (TextBox)sender;
				while(CurrentTextbox.Text.Length != 2)
					CurrentTextbox.Text = ("0" + CurrentTextbox.Text);			
			}				
		}

		private void chbExtended_CheckedChanged(object sender, System.EventArgs e)
		{
			uint uiTemp;

			txtID.MaxLength = (chbExtended.Checked)? 8: 3;
			
			// the only way that the text length can be bigger als MaxLength
			// is when the change is from Extended to Standard message Type.
			// We have to handle this and set an ID not bigger than the Maximum
			// ID value for a Standard Message (0x7FF)
			//
			if(txtID.Text.Length > txtID.MaxLength)
			{
				uiTemp = Convert.ToUInt32(txtID.Text,16);
				txtID.Text = (uiTemp < 0x7FF) ?  string.Format("{0:X3}",uiTemp): "7FF";
			}

			txtID_Leave(this,new EventArgs());			
		}

		private void chbRemote_CheckedChanged(object sender, System.EventArgs e)
		{
			TextBox CurrentTextBox;

			CurrentTextBox = txtData0;

			// We enable so much TextBox Data fields as the length of the
			// message will be, that is the value of the UpDown control.
			// 
			for(int i=0; i< 8; i++)
			{
				CurrentTextBox.Visible = !chbRemote.Checked;
				if(i < 7)
					CurrentTextBox = (TextBox)this.GetNextControl(CurrentTextBox,true);
			}			
		}

        private void rdbStandard_CheckedChanged(object sender, EventArgs e)
        {
            uint uiTemp;

            txtIdFrom.MaxLength = (rdbExtended.Checked) ? 8 : 3;
            txtIdTo.MaxLength = txtIdFrom.MaxLength;

            // the only way that the text length can be bigger als MaxLength
            // is when the change is from Extended to Standard message Type.
            // We have to handle this and set an ID not bigger than the Maximum
            // ID value for a Standard Message (0x7FF)
            //
            if (txtIdFrom.Text.Length > txtIdFrom.MaxLength)
            {
                uiTemp = Convert.ToUInt32(txtIdFrom.Text,16);
                txtIdFrom.Text = (uiTemp < 0x7FF) ? string.Format("{0:X3}", uiTemp) : "7FF";
            }
            if (txtIdTo.Text.Length > txtIdTo.MaxLength)
            {
                uiTemp = Convert.ToUInt32(txtIdTo.Text,16);
                txtIdTo.Text = (uiTemp < 0x7FF) ? string.Format("{0:X3}", uiTemp) : "7FF";
            }
        }

        private void txtIdFrom_Leave(object sender, EventArgs e)
        {
            int TextLength;
            uint MaxValue;
            TextBox IdBox;

            IdBox = sender as TextBox;
            // calculate the text length and Maximum ID value according
            // with the Message Type
            //
            TextLength = (rdbExtended.Checked) ? 8 : 3;
            MaxValue = (rdbExtended.Checked) ? (uint)0x1FFFFFFF : (uint)0x7FF;

            // The Textbox for the ID is represented with 3 characters for 
            // Standard and 8 characters for extended messages.
            // Therefore if the Length of the text is smaller than TextLength,  
            // we add "0"
            //
            while (IdBox.Text.Length != TextLength)
                IdBox.Text = ("0" + IdBox.Text);

            // Because in this example will be sent only Standard messages
            // we check that the ID is not bigger than 0x7FF
            //
            if (Convert.ToUInt32(IdBox.Text,16) > MaxValue)
                IdBox.Text = string.Format("{0:X" + TextLength.ToString() + "}", MaxValue);
        }

		private void txtInfo_DoubleClick(object sender, System.EventArgs e)
		{
			// We clear the Information edit box
			//
			txtInfo.Text = "";		
		}

		private void lstMessages_DoubleClick(object sender, System.EventArgs e)
		{
			lock(this)
			{
				lstMessages.Items.Clear();
				LastMsgsList.Clear();
			}		
		}

		private void btnInfo_Click(object sender, System.EventArgs e)
		{
			string strInfo;
			CANResult Res;

			// We execute the "VersionInfo" function of the PCANLight 
			// using as parameter the Hardware type and a string 
			// variable to get the info.
			// 
            Res = PCANLight.VersionInfo((HardwareType)cbbHws.SelectedIndex, out strInfo);
			strInfo = strInfo.Replace("\n","\r\n");

			// The function was successfully executed
			//			
			if(Res == CANResult.ERR_OK)
				// We show the Version Information
				//
				txtInfo.Text = strInfo;
				// An error occurred.  We show the error.
				//			
			else
				txtInfo.Text  = "Error: " + Res.ToString();			
		}

		private void btnWrite_Click(object sender, System.EventArgs e)
		{
			TCLightMsg MsgToSend;
			TextBox CurrentTextBox;		
			CANResult Res;

			// We create a TCLightMsg message structure 
			//
			MsgToSend = new TCLightMsg();

			// We configurate the Message.  The ID (max 0x1FF),
			// Length of the Data, Message Type (Standard in 
			// this example) and die data
			//
			MsgToSend.ID = Convert.ToUInt32(txtID.Text,16);
			MsgToSend.Len = Convert.ToByte(nudLength.Value);
			MsgToSend.MsgType = (chbExtended.Checked) ? MsgTypes.MSGTYPE_EXTENDED : MsgTypes.MSGTYPE_STANDARD;
			// If a remote frame will be sent, the data bytes are not important.
			//
			if(chbRemote.Checked)
				MsgToSend.MsgType |= MsgTypes.MSGTYPE_RTR;
			else
			{
				// We get so much data as the Len of the message
				//
				CurrentTextBox = txtData0;
				for(int i=0; i < MsgToSend.Len; i++)
				{
					MsgToSend.Data[i] = Convert.ToByte(CurrentTextBox.Text,16);
					if(i < 7)
						CurrentTextBox = (TextBox)this.GetNextControl(CurrentTextBox,true);
				}
			}

			// The message is sent to the configured hardware
			//
            Res = PCANLight.Write(ActiveHardware, MsgToSend);
			
			// The Hardware was successfully sent
			//
			if(Res == CANResult.ERR_OK)
				txtInfo.Text = "Message was successfully SENT.\r\n";
				// An error occurred.  We show the error.
				//			
			else
				txtInfo.Text = "Error: " + Res.ToString();		
		}
		
        private void btnSetFilter_Click(object sender, EventArgs e)
        {
            uint FromID, ToID;
            CANResult Res;

            // The range IDs is read
            //
            FromID = Convert.ToUInt32(txtIdFrom.Text,16);
            ToID = Convert.ToUInt32(txtIdTo.Text,16);
            
            // The desired Filter is set on the configured Hardware
            //
            Res = PCANLight.MsgFilter(ActiveHardware, FromID, ToID, (rdbStandard.Checked) ? MsgTypes.MSGTYPE_STANDARD : MsgTypes.MSGTYPE_EXTENDED);

            // The Filter was successfully set
            //
            if (Res == CANResult.ERR_OK)
                txtInfo.Text = "Filter was successfully SET.\r\n";
            // An error occurred.  We show the error.
            //			
            else
                txtInfo.Text = "Error: " + Res.ToString();		
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            CANResult Res;

            // The current Filter on the configured Hardware is reset 
            //
            Res = PCANLight.ResetFilter(ActiveHardware);

            // The Filter was successfully reset
            //
            if (Res == CANResult.ERR_OK)
                txtInfo.Text = "Filter was successfully RESET.\r\n";
            // An error occurred.  We show the error.
            //			
            else
                txtInfo.Text = "Error: " + Res.ToString();		
        }

        private void btnGetUsbDevNumber_Click(object sender, EventArgs e)
        {
            uint iDevNum;
            CANResult Res;

            // The USB Device Number will asked 
            //
            Res = PCANLight.GetUSBDeviceNr((HardwareType)cbbHws.SelectedIndex, out iDevNum);

            // The Device number was got successfully
            //
            if (Res == CANResult.ERR_OK)
                MessageBox.Show("USB Device Number is: " + iDevNum, "GetUSBDevNr");
            // An error occurred.  We show the error.
            //			
            else
                MessageBox.Show("Get USB Device Number failed: " + Res.ToString(), "GetUSBDevNr"); 	
        }

        private void btnSetUsbDevNumber_Click(object sender, EventArgs e)
        {
            CANResult Res;

            // The USB Device Number will set 
            //
            Res = PCANLight.SetUSBDeviceNr((HardwareType)cbbHws.SelectedIndex, Convert.ToUInt32(txtDevNumber.Text));

            // The Device number was set successfully
            //
            if (Res == CANResult.ERR_OK)
                MessageBox.Show("USB Device Number was set", "SetUSBDevNr");
            // An error occurred.  We show the error.
            //
            else
                MessageBox.Show("Set USB Device Number failed: " + Res.ToString(), "SetUSBDevNr");	
        }

        private void btnDllInfo_Click(object sender, EventArgs e)
        {
            String strInfo = "";
            CANResult Res;

            // We execute the "VersionInfo" function of the PCANLight 
            // using as parameter the Hardware type and a string 
            // variable to get the info.
            // 
            Res = PCANLight.DllVersionInfo((HardwareType)cbbHws.SelectedIndex, out strInfo);

            // The function was successfully executed
            //			
            if (Res == CANResult.ERR_OK)
            {
                // We show the Version Information
                //
                strInfo = strInfo.Replace("\n", "\r\n");
                strInfo = cbbHws.SelectedItem.ToString() + " Dll Version: " + strInfo;
                txtInfo.Text = strInfo;
            }
            // An error occurred.  We show the error.
            //			
            else
                txtInfo.Text = "Error: " + String.Format("{0:X4}", (int)Res);
        }

        private void rdbTimer_CheckedChanged(object sender, EventArgs e)
        {
            if(rdbTimer.Checked)
		    {
			    // Abort Read Thread if it exists
			    //
			    if(ReadThread != null)
			    {
				    ReadThread.Abort();
				    ReadThread.Join();
				    ReadThread = null;
			    }

			    // Enable Timer
			    //
			    tmrRead.Enabled = true;
		    }
		    else
		    {
			    // Disable Timer
			    //
			    tmrRead.Enabled = false;
			    // Create and start the tread to read CAN Message using SetRcvEvent()
			    ThreadStart threadDelegate = new ThreadStart(this.CANReadThreadFunc);
			    ReadThread = new Thread(threadDelegate);
			    ReadThread.Start();
		    }
        }

        private void chbTimeStamp_CheckedChanged(object sender, EventArgs e)
        {
            if (chbTimeStamp.Checked)
            {
                // Add Rcv Time column
                //
                if (!lstMessages.Columns.Contains(clhRcvTime))
                    lstMessages.Columns.Add(clhRcvTime);
            }
            else
            {
                // Remove Rcv Time column
                //
                if (lstMessages.Columns.Contains(clhRcvTime))
                    lstMessages.Columns.Remove(clhRcvTime);
            }
        }

        private void txtDevNumber_Leave(object sender, EventArgs e)
        {
            if(txtDevNumber.Text == "")
                txtDevNumber.Text = "0";
            if (Convert.ToUInt64(txtDevNumber.Text) > uint.MaxValue)
                txtDevNumber.Text = uint.MaxValue.ToString();
        }

        private void txtDevNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // The Key is the Delete (Backspace) Key
            //
            if (e.KeyChar == 8)
                return;
            // The Key is a number between 0-9
            //
            if ((e.KeyChar > 47) && (e.KeyChar < 58))
                return;

            // Is neither a number nor a character between A(a) and F(f)
            //
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}