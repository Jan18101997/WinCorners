using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;

namespace WinCorners
{
    public class Screen
    {
        public List<HotCorner> Corners { get; set; } = new List<HotCorner>();

        public string ScreendID;

        public double ScreenHeight;

        public double ScreenWidth;

        public ScreenPosition ScreenPosition;

        public static bool SaveScreens(List<Screen> screens)
        {
            try
            {
                XmlWriter writer = XmlWriter.Create("Corners.xml", new XmlWriterSettings() { Indent = true });

                writer.WriteStartDocument();
                {
                    writer.WriteStartElement("WinCorners");
                    {
                        foreach (Screen screen in screens)
                        {
                            writer.WriteStartElement("Screen");
                            {
                                writer.WriteAttributeString("ScreenID", screen.ScreendID);
                                writer.WriteAttributeString("ScreenHeight", screen.ScreenWidth.ToString());
                                writer.WriteAttributeString("ScreenWidth", screen.ScreenHeight.ToString());
                                writer.WriteAttributeString("ScreenPosition", screen.ScreenPosition.ToSaveString());

                                foreach (HotCorner item in screen.Corners)
                                {
                                    writer.WriteStartElement("Corner");
                                    {
                                        writer.WriteAttributeString("pos1", item.Position1.X + "," + item.Position1.Y);
                                        writer.WriteAttributeString("pos2", item.Position2.X + "," + item.Position2.Y);
                                        writer.WriteAttributeString("runonce", item.RunOnce.ToString());
                                        writer.WriteAttributeString("disabableatmousedown", item.DisableAtMouseDown.ToString());
                                        writer.WriteAttributeString("commandtype", item.Command.GetType().ToString());
                                        writer.WriteAttributeString("command", item.Command.ToSaveString());
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LoadScreens(out List<Screen> screens)
        {
            screens = new List<Screen>();

            if (File.Exists("Corners.xml") == false)
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Corners.xml");

                XmlNodeList screenList = doc.SelectNodes("/WinCorners/Screen");
                if (screenList != null)
                    foreach (XmlNode screenNode in screenList)
                    {
                        Screen screen = new Screen();

                        screen.ScreendID = screenNode.Attributes["ScreenID"].InnerText;
                        screen.ScreenWidth = double.Parse(screenNode.Attributes["ScreenHeight"].InnerText);
                        screen.ScreenHeight = double.Parse(screenNode.Attributes["ScreenWidth"].InnerText);
                        screen.ScreenPosition = ScreenPosition.FromSaveString(screenNode.Attributes["ScreenPosition"].InnerText);

                        XmlNodeList cornerList = screenNode.SelectNodes("Corner");
                        if (cornerList != null)
                            foreach (XmlNode cornerNode in cornerList)
                            {
                                HotCorner corner = new HotCorner();

                                corner.Position1 = Point.Parse(cornerNode.Attributes["pos1"].InnerText);
                                corner.Position2 = Point.Parse(cornerNode.Attributes["pos2"].InnerText);
                                corner.DisableAtMouseDown = bool.Parse(cornerNode.Attributes["disabableatmousedown"].InnerText);
                                corner.RunOnce = bool.Parse(cornerNode.Attributes["runonce"].InnerText);

                                Type commandType = Type.GetType(cornerNode.Attributes["commandtype"].InnerText);

                                corner.Command = (ICommand)Activator.CreateInstance(commandType);

                                corner.Command.FromSaveString(cornerNode.Attributes["command"].InnerText);

                                screen.Corners.Add(corner);
                            }

                        screens.Add(screen);
                    }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}