﻿using KinectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace KinectAlarm
{
    public static class ActionCollection
    {
        static List<Kinect.Joint[]> actionList = new List<Kinect.Joint[]>();

        public static void AddAction(Kinect.Joint[] action)
        {
            actionList.Add(action);
        }

        public static void RemoveAction(int index)
        {
            actionList.RemoveAt(index);
        }

        public static IReadOnlyList<Kinect.Joint[]> ActionList { get { return actionList; } }

        public static async void LoadData()
        {
            actionList.Clear();
            try
            {
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync("actionList.dat");
                using (IRandomAccessStream raStream = await storageFile.OpenAsync(FileAccessMode.Read))
                {
                    DataReader reader = new DataReader(raStream);
                    int dataLength = reader.ReadInt32();
                    for (int i = 0; i < dataLength; i++)
                    {
                        Kinect.Joint[] action = new Kinect.Joint[20];
                        for (int j = 0; j < 20; j++)
                        {
                            Kinect.Joint joint = new Kinect.Joint();
                            joint.JointType = (Kinect.JointType)reader.ReadByte();
                            joint.X = reader.ReadSingle();
                            joint.Y = reader.ReadSingle();
                            joint.Z = reader.ReadSingle();
                            action[j] = joint;
                        }
                        actionList.Add(action);
                    }
                }
            }
            catch { }
        }

        public static async void SaveData()
        {
            StorageFile storageFile = await StorageFile.CreateStreamedFileAsync("actionList.dat", null, null);
            using (IRandomAccessStream raStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                DataWriter writer = new DataWriter(raStream);
                writer.WriteInt32(actionList.Count);
                foreach (Kinect.Joint[] action in actionList)
                {
                    foreach (Kinect.Joint joint in action)
                    {
                        writer.WriteByte((byte)joint.JointType);
                        writer.WriteSingle(joint.X);
                        writer.WriteSingle(joint.Y);
                        writer.WriteSingle(joint.Z);
                    }
                }
                await writer.FlushAsync();
            }
        }
    }
}