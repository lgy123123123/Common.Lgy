using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Common.CustomException;
using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// xml序列化帮助类
    /// </summary>
    public static class XmlUtil
    {
        private static XmlDocument doc { get; set; } = new XmlDocument();

        /// <summary>
        /// 将xml节点转成模型
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="xml">xml节点</param>
        /// <returns>模型</returns>
        public static T XmlToModel<T>(string xml) where T : class
        {
            try
            {
                using (StringReader strReader = new StringReader(xml))
                {
                    XmlSerializer xSerializer = new XmlSerializer(typeof(T));
                    return (T)xSerializer.Deserialize(strReader);
                }
            }
            catch (Exception e)
            {
                throw new BaseException("xml反序列化错误:" + e.Message);
            }
        }

        /// <summary>
        /// 将xml节点转成模型
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="stream">xml流</param>
        /// <returns>模型</returns>
        public static T XmlToModel<T>(Stream stream) where T : class
        {
            try
            {
                XmlSerializer xSerializer = new XmlSerializer(typeof(T));
                return (T)xSerializer.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new BaseException("xml反序列化错误:" + e.Message);
            }
        }

        /// <summary>
        /// 模型转xml
        /// </summary>
        /// <typeparam name="T">模型类</typeparam>
        /// <param name="model">模型实体</param>
        /// <returns>XmlNode节点</returns>
        public static XmlNode ModelToXml<T>(T model) where T : class
        {
            try
            {
                MemoryStream memory = new MemoryStream();
                XmlSerializer xSerializer = new XmlSerializer(typeof(T));
                xSerializer.Serialize(memory, model);
                memory.Position = 0;
                using (StreamReader sReader = new StreamReader(memory))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(sReader);
                    return xmlDoc;
                }
            }
            catch (Exception e)
            {
                throw new BaseException(e.Message);
            }
        }

        /// <summary>
        /// 将xml转成json字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>json字符串</returns>
        public static string XmlToJson(XmlNode xml)
        {
            return JsonConvert.SerializeXmlNode(xml);
        }

        /// <summary>
        /// json字符串转xml(属性)
        /// </summary>
        /// <param name="json"></param>
        /// <returns>XmlNode节点</returns>
        public static XmlNode JsonToXml(string json)
        {
            return JsonConvert.DeserializeXmlNode(json);
        }

        /// <summary>
        /// 加载xml文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlDocument LoadByPath(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc;
        }

        /// <summary>
        /// 加载xml字符串
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlDocument LoadByString(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        /// <summary>
        /// 保存xml
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="path">xml路径</param>
        /// <param name="xmlModel">保存的类</param>
        public static void SaveXml<T>(string path, T xmlModel) where T : class
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ModelToXml(xmlModel).OuterXml);
            doc.Save(path);
        }
        /// <summary>
        /// 设置节点属性
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="name">属性名</param>
        /// <param name="value">值</param>
        public static void SetXmlNodeAttribute(XmlNode node, string name, string value)
        {
            if (node.Attributes[name] == null)
            {
                XmlAttribute xAttr = doc.CreateAttribute(name);
                node.Attributes.Append(xAttr);
            }
            node.Attributes[name].Value = value;
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="strAttributeName">属性名</param>
        /// <returns></returns>
        public static string GetXmlNodeAttribute(XmlNode node, string strAttributeName)
        {
            if (node.Attributes[strAttributeName] == null)
            {
                return "";
            }
            return node.Attributes[strAttributeName].Value.Trim();
        }
    }
    /// <summary>
    /// xml序列化cdata时，作为一个节点，使用时，标签打上[XmlElement("name")]
    /// </summary>
    public class CData : IXmlSerializable
    {
        private string _Value;
        /// <summary>
        /// 
        /// </summary>
        public CData()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_Value"></param>
        public CData(string p_Value)
        {
            _Value = p_Value;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get
            {
                return _Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            this._Value = reader.ReadElementContentAsString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteCData(_Value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return (null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator string(CData element)
        {
            return (element == null) ? null : element._Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static implicit operator CData(string text)
        {
            return new CData(text);
        }
    }

    /// <summary>
    /// xml序列化用的Dictionary
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public XmlDictionary()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        public XmlDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        public XmlDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public XmlDictionary(int capacity) : base(capacity)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public XmlDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected XmlDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
        #endregion 构造函数

        #region IXmlSerializable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() => null;

        /// <summary>
        ///     从对象的 XML 表示形式生成该对象(反序列化)
        /// </summary>
        /// <param name="xr"></param>
        public void ReadXml(XmlReader xr)
        {
            if (xr.IsEmptyElement)
                return;
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            xr.Read();
            while (xr.NodeType != XmlNodeType.EndElement)
            {
                xr.ReadStartElement("Item");
                xr.ReadStartElement("Key");
                var key = (TKey)ks.Deserialize(xr);
                xr.ReadEndElement();
                xr.ReadStartElement("Value");
                var value = (TValue)vs.Deserialize(xr);
                xr.ReadEndElement();
                Add(key, value);
                xr.ReadEndElement();
                xr.MoveToContent();
            }
            xr.ReadEndElement();
        }

        /// <summary>
        ///     将对象转换为其 XML 表示形式(序列化)
        /// </summary>
        /// <param name="xw"></param>
        public void WriteXml(XmlWriter xw)
        {
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            foreach (var key in Keys)
            {
                xw.WriteStartElement("Item");
                xw.WriteStartElement("Key");
                ks.Serialize(xw, key);
                xw.WriteEndElement();
                xw.WriteStartElement("Value");
                vs.Serialize(xw, this[key]);
                xw.WriteEndElement();
                xw.WriteEndElement();
            }
        }
        #endregion IXmlSerializable Members
    }
}
