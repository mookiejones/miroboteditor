﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5483
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class configuration {
    
    private configurationAuthenticationManagement authenticationManagementField;
    
    private configurationSecondaryUserList secondaryUserListField;
    
    /// <remarks/>
    public configurationAuthenticationManagement authenticationManagement {
        get {
            return this.authenticationManagementField;
        }
        set {
            this.authenticationManagementField = value;
        }
    }
    
    /// <remarks/>
    public configurationSecondaryUserList secondaryUserList {
        get {
            return this.secondaryUserListField;
        }
        set {
            this.secondaryUserListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationAuthenticationManagement {
    
    private configurationAuthenticationManagementAuthenticationConfiguration authenticationConfigurationField;
    
    /// <remarks/>
    public configurationAuthenticationManagementAuthenticationConfiguration AuthenticationConfiguration {
        get {
            return this.authenticationConfigurationField;
        }
        set {
            this.authenticationConfigurationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationAuthenticationManagementAuthenticationConfiguration {
    
    private string autoLockField;
    
    private string systemLockModeField;
    
    private configurationAuthenticationManagementAuthenticationConfigurationAuthenticationMethod[] methodListField;
    
    private configurationAuthenticationManagementAuthenticationConfigurationDefaultUser defaultUserField;
    
    private ushort announcementTimeField;
    
    /// <remarks/>
    public string AutoLock {
        get {
            return this.autoLockField;
        }
        set {
            this.autoLockField = value;
        }
    }
    
    /// <remarks/>
    public string SystemLockMode {
        get {
            return this.systemLockModeField;
        }
        set {
            this.systemLockModeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("AuthenticationMethod", IsNullable=false)]
    public configurationAuthenticationManagementAuthenticationConfigurationAuthenticationMethod[] MethodList {
        get {
            return this.methodListField;
        }
        set {
            this.methodListField = value;
        }
    }
    
    /// <remarks/>
    public configurationAuthenticationManagementAuthenticationConfigurationDefaultUser DefaultUser {
        get {
            return this.defaultUserField;
        }
        set {
            this.defaultUserField = value;
        }
    }
    
    /// <remarks/>
    public ushort AnnouncementTime {
        get {
            return this.announcementTimeField;
        }
        set {
            this.announcementTimeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationAuthenticationManagementAuthenticationConfigurationAuthenticationMethod {
    
    private string authenticationTypeField;
    
    private string userListNameField;
    
    private ushort leaseTimeField;
    
    /// <remarks/>
    public string AuthenticationType {
        get {
            return this.authenticationTypeField;
        }
        set {
            this.authenticationTypeField = value;
        }
    }
    
    /// <remarks/>
    public string UserListName {
        get {
            return this.userListNameField;
        }
        set {
            this.userListNameField = value;
        }
    }
    
    /// <remarks/>
    public ushort LeaseTime {
        get {
            return this.leaseTimeField;
        }
        set {
            this.leaseTimeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationAuthenticationManagementAuthenticationConfigurationDefaultUser {
    
    private string nameField;
    
    private byte userLevelField;
    
    private string displayNameField;
    
    private bool translateDisplayNameField;
    
    private string passwordField;
    
    private bool usePasswordAutomaticalyField;
    
    /// <remarks/>
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public byte UserLevel {
        get {
            return this.userLevelField;
        }
        set {
            this.userLevelField = value;
        }
    }
    
    /// <remarks/>
    public string DisplayName {
        get {
            return this.displayNameField;
        }
        set {
            this.displayNameField = value;
        }
    }
    
    /// <remarks/>
    public bool TranslateDisplayName {
        get {
            return this.translateDisplayNameField;
        }
        set {
            this.translateDisplayNameField = value;
        }
    }
    
    /// <remarks/>
    public string Password {
        get {
            return this.passwordField;
        }
        set {
            this.passwordField = value;
        }
    }
    
    /// <remarks/>
    public bool UsePasswordAutomaticaly {
        get {
            return this.usePasswordAutomaticalyField;
        }
        set {
            this.usePasswordAutomaticalyField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationSecondaryUserList {
    
    private configurationSecondaryUserListUserInfo[] userListField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("UserInfo", IsNullable=false)]
    public configurationSecondaryUserListUserInfo[] UserList {
        get {
            return this.userListField;
        }
        set {
            this.userListField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class configurationSecondaryUserListUserInfo {
    
    private string nameField;
    
    private byte userLevelField;
    
    private string displayNameField;
    
    private bool translateDisplayNameField;
    
    private string passwordField;
    
    private bool usePasswordAutomaticalyField;
    
    private bool usePasswordAutomaticalyFieldSpecified;
    
    /// <remarks/>
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public byte UserLevel {
        get {
            return this.userLevelField;
        }
        set {
            this.userLevelField = value;
        }
    }
    
    /// <remarks/>
    public string DisplayName {
        get {
            return this.displayNameField;
        }
        set {
            this.displayNameField = value;
        }
    }
    
    /// <remarks/>
    public bool TranslateDisplayName {
        get {
            return this.translateDisplayNameField;
        }
        set {
            this.translateDisplayNameField = value;
        }
    }
    
    /// <remarks/>
    public string Password {
        get {
            return this.passwordField;
        }
        set {
            this.passwordField = value;
        }
    }
    
    /// <remarks/>
    public bool UsePasswordAutomaticaly {
        get {
            return this.usePasswordAutomaticalyField;
        }
        set {
            this.usePasswordAutomaticalyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool UsePasswordAutomaticalySpecified {
        get {
            return this.usePasswordAutomaticalyFieldSpecified;
        }
        set {
            this.usePasswordAutomaticalyFieldSpecified = value;
        }
    }
}
