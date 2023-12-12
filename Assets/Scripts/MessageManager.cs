using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace PlayTheHappiness
//{
/// <summary>
/// MessageManager
/// Responsible for the messages passed between objects
/// </summary>
public enum MessageType {
    LoadingFinished,
    GameOver,
    LevelSelected,
    Wall,
    PortalPending,
    PortalCreated,
    Star,
    EndPortal
}

interface ISubsribe {
    void MessageReceived (MessageType message, GameObject sender);
}

public static class MessageManager {
    public static event Action<MessageType, GameObject> onMessage;
    public static event Action<MessageType, GameObject, GameObject> onObjectMessage;
    public static event Action<MessageType, int, GameObject> onIntMessage;
    public static event Action<MessageType, Vector3, GameObject> onVector3Message;
    public static event Action<MessageType, Portal, GameObject> onPortalMessage;

    public static void SendMessage (MessageType message, GameObject sender) {
        onMessage (message, sender);
    }
    public static void SendMessage (MessageType message, GameObject receiver, GameObject sender) {
        onObjectMessage (message, receiver, sender);
    }
    public static void SendMessage (MessageType message, int value, GameObject sender) {
        onIntMessage (message, value, sender);
    }
    public static void SendMessage (MessageType message, Vector3 value, GameObject sender) {
        onVector3Message (message, value, sender);
    }
    public static void SendMessage (MessageType message, Portal value, GameObject sender) {
        onPortalMessage (message, value, sender);
    }
}
//}