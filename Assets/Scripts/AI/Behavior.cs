﻿using UnityEngine;
using System;
using System.Xml;
using System.Reflection;
using System.Collections;

public class Behavior : MonoBehaviour
{

    public enum BehaviorTypes
    {
        DragonBasic,
        PlayerBasic
    }

    public XmlDocument ReadFile(string fileName)
    {
        UnityEngine.Object dragonBasic = Resources.Load("AI/Behaviours/" + fileName);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(dragonBasic.ToString());
        return xml;
    }

    public IBTNode GetTree(BehaviorTypes type)
    {
        XmlDocument xml = ReadFile(Enum.GetName(typeof(BehaviorTypes), type));
        return CreateTreeFromXML(xml.ChildNodes);
        //return null;
    }

    private bool IsAcceptableNodeName(string nodeName)
    {
        switch (nodeName)
        {
            case "xml":
                return false;
            case "type":
                return false;
        }
        return true;
    }

    private IBTNode CreateTreeFromXML(XmlNodeList xmlChildren)
    {
        IBTSequencer btNode = new BTSequencer();

        foreach (XmlNode node in xmlChildren)
        {
            if (node.ChildNodes.Count > 0)
            {
                IBTNode childrenNode = CreateTreeFromXML(node.ChildNodes);
                btNode.AppendNode(CreateBtNode(node, childrenNode));
            }
            else
            {
                btNode.AppendNode(CreateBtNode(node));
            }

        }
        return btNode;
    }

    private IBTNode CreateBtNode(XmlNode node, IBTNode children)
    {
        IBTNode btNode = null;
        switch (node.Name)
        {
            case "sequence":
                btNode = CreateSequence(node).AppendNode(children);
                break;
            case "inverter":
                btNode = CreateInverter(node).SetNode(children);
                break;
            case "positiveSequence":
                btNode = CreatePositiveSequence(node).AppendNode(children);
                break;
        }
        return btNode;
    }

    private IBTNode CreateBtNode(XmlNode node)
    {
        IBTNode btNode = null;
        switch (node.Name)
        {
            case "action":
                btNode = CreateAction(node);
                break;
            case "condition":
                btNode = CreateCondition(node);
                break;

        }
        return btNode;
    }

    private IBTCondition CreateCondition(XmlNode node)
    {
        string conditionClassName = node.Attributes["name"].Value;
        //Debug.Log("Creating condition " + conditionClassName);
        IBTNode btNode = (IBTNode) CreateReflectionObject(conditionClassName);
        return (IBTCondition)btNode;
    }
    private IBTAction CreateAction(XmlNode node)
    {
        string actionClassName = node.Attributes["name"].Value;

        //Debug.Log("Creating Action " + actionClassName);

        return (IBTAction)CreateReflectionObject(actionClassName);
    }
    private IBTSequencer CreateSequence(XmlNode node)
    {
        //string sequenceClassName = node.Attributes["name"].Value;
        return (IBTSequencer)CreateReflectionObject("BTSequencer");
    }
    private IBTPositiveSequencer CreatePositiveSequence(XmlNode node)
    {
        //string positiveSequencerClassName = node.Attributes["name"].Value;
        return (IBTPositiveSequencer)CreateReflectionObject("BTPositiveSequencer");
    }
    private IBTInvertor CreateInverter(XmlNode node)
    {
        //string inverterClassName = node.Attributes["name"].Value;
        return (IBTInvertor)CreateReflectionObject("BTInvertor");
    }
    private IBTSelector CreateSelector(XmlNode node)
    {
        //string selectorClassName = node.Attributes["name"].Value;
        return (IBTSelector)CreateReflectionObject("BTSelector");
    }

    private object CreateReflectionObject(string clazz)
    {
        Type t = Type.GetType(clazz);
        return Activator.CreateInstance(t);
    }
}








