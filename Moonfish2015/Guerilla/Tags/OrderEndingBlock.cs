using Moonfish.Model;
using Moonfish.Tags.BlamExtension;
using Moonfish.Tags;
using OpenTK;
using System;
using System.IO;

namespace Moonfish.Guerilla.Tags
{
    public  partial class OrderEndingBlock : OrderEndingBlockBase
    {
        public  OrderEndingBlock(BinaryReader binaryReader): base(binaryReader)
        {
            
        }
    };
    [LayoutAttribute(Size = 20)]
    public class OrderEndingBlockBase
    {
        internal Moonfish.Tags.ShortBlockIndex1 nextOrder;
        internal CombinationRule combinationRule;
        internal float delayTime;
        /// <summary>
        /// when this ending is triggered, launch a dialogue event of the given type
        /// </summary>
        internal DialogueTypeWhenThisEndingIsTriggeredLaunchADialogueEventOfTheGivenType dialogueType;
        internal byte[] invalidName_;
        internal TriggerReferences[] triggers;
        internal  OrderEndingBlockBase(BinaryReader binaryReader)
        {
            this.nextOrder = binaryReader.ReadShortBlockIndex1();
            this.combinationRule = (CombinationRule)binaryReader.ReadInt16();
            this.delayTime = binaryReader.ReadSingle();
            this.dialogueType = (DialogueTypeWhenThisEndingIsTriggeredLaunchADialogueEventOfTheGivenType)binaryReader.ReadInt16();
            this.invalidName_ = binaryReader.ReadBytes(2);
            this.triggers = ReadTriggerReferencesArray(binaryReader);
        }
        internal  virtual byte[] ReadData(BinaryReader binaryReader)
        {
            var blamPointer = binaryReader.ReadBlamPointer(1);
            var data = new byte[blamPointer.Count];
            if(blamPointer.Count > 0)
            {
                using (binaryReader.BaseStream.Pin())
                {
                    binaryReader.BaseStream.Position = blamPointer[0];
                    data = binaryReader.ReadBytes(blamPointer.Count);
                }
            }
            return data;
        }
        internal  virtual TriggerReferences[] ReadTriggerReferencesArray(BinaryReader binaryReader)
        {
            var elementSize = Deserializer.SizeOf(typeof(TriggerReferences));
            var blamPointer = binaryReader.ReadBlamPointer(elementSize);
            var array = new TriggerReferences[blamPointer.Count];
            using (binaryReader.BaseStream.Pin())
            {
                for (int i = 0; i < blamPointer.Count; ++i)
                {
                    binaryReader.BaseStream.Position = blamPointer[i];
                    array[i] = new TriggerReferences(binaryReader);
                }
            }
            return array;
        }
        internal enum CombinationRule : short
        
        {
            OR = 0,
            AND = 1,
        };
        internal enum DialogueTypeWhenThisEndingIsTriggeredLaunchADialogueEventOfTheGivenType : short
        
        {
            None = 0,
            Advance = 1,
            Charge = 2,
            FallBack = 3,
            Retreat = 4,
            Moveone = 5,
            Arrival = 6,
            EnterVehicle = 7,
            ExitVehicle = 8,
            FollowPlayer = 9,
            LeavePlayer = 10,
            Support = 11,
        };
    };
}
