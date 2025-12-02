using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMe.Models.Enums;


    public enum ConversationType { Private = 0, Group = 1 }
    public enum MessageType { Text = 0, Image = 1, Video = 2, File = 3 }
    public enum GroupRole { Member = 0, Admin = 1, Owner = 2 }

