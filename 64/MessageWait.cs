using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICLRead
{

    // ********************************************************************************
    // CAN MESSAGE DEFINITIONS
    // -------------------------------------------------------------------------------
    // Messages are constructed as follows:
    // Base Message ID = [priority]|[type]|[group]|[message_id]|[dest_id]|[src_id]
    // ********************************************************************************
    // Message Name                    Encoding      Primary Type     Group           Dest      #Bytes/Description
    // ----------------------------------------------------------------------------------------------------------------------------------------------
    public enum MessageList
    {
        CAN_BTLD_READY = 0x02F85BC0,   //Announcement   Data_Transfer   Peers      (4  ) Bootloader is running
        CAN_BLK_SEND_SEG = 0x01783800,   //Command        Data_Transfer   Null       (0  ) send the next segment
    }

    class MessageWait
    {
        


    }
}
