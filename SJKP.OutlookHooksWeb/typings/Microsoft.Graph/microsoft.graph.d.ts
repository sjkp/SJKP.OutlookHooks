
declare module Microsoft.Graph {
    interface directoryObject extends Microsoft.OData.Client.BaseEntityType {
        id: string;
    }
    interface appRoleAssignment extends Microsoft.OData.Client.BaseEntityType {
        creationTimestamp: Date;
        id: System.Guid;
        principalDisplayName: string;
        principalId: System.Guid;
        principalType: string;
        resourceDisplayName: string;
        resourceId: System.Guid;
    }
    interface conversationThread extends Microsoft.OData.Client.BaseEntityType {
        toRecipients: Microsoft.Graph.recipient[];
        topic: string;
        hasAttachments: boolean;
        lastDeliveredDateTime: Date;
        uniqueSenders: string[];
        ccRecipients: Microsoft.Graph.recipient[];
        preview: string;
        isLocked: boolean;
        id: string;
        posts: Microsoft.Graph.post[];
    }
    interface recipient {
        emailAddress: Microsoft.Graph.emailAddress;
    }
    interface emailAddress {
        name: string;
        address: string;
    }
    interface post extends Microsoft.Graph.outlookItem {
        body: Microsoft.Graph.itemBody;
        receivedDateTime: Date;
        hasAttachments: boolean;
        from: Microsoft.Graph.recipient;
        sender: Microsoft.Graph.recipient;
        conversationThreadId: string;
        newParticipants: Microsoft.Graph.recipient[];
        conversationId: string;
        extensions: Microsoft.Graph.extension[];
        inReplyTo: Microsoft.Graph.post;
        attachments: Microsoft.Graph.attachment[];
    }
    interface outlookItem extends Microsoft.OData.Client.BaseEntityType {
        createdDateTime: Date;
        lastModifiedDateTime: Date;
        changeKey: string;
        categories: string[];
        id: string;
    }
    interface itemBody {
        contentType: Microsoft.Graph.bodyType;
        content: string;
    }
    interface extension extends Microsoft.OData.Client.BaseEntityType {
        id: string;
    }
    interface attachment extends Microsoft.OData.Client.BaseEntityType {
        lastModifiedDateTime: Date;
        name: string;
        contentType: string;
        size: number;
        isInline: boolean;
        id: string;
    }
    interface calendar extends Microsoft.OData.Client.BaseEntityType {
        name: string;
        color: Microsoft.Graph.calendarColor;
        changeKey: string;
        id: string;
        events: Microsoft.Graph.event[];
        calendarView: Microsoft.Graph.event[];
    }
    interface event extends Microsoft.Graph.outlookItem {
        originalStartTimeZone: string;
        originalEndTimeZone: string;
        responseStatus: Microsoft.Graph.responseStatus;
        iCalUId: string;
        reminderMinutesBeforeStart: number;
        isReminderOn: boolean;
        hasAttachments: boolean;
        subject: string;
        body: Microsoft.Graph.itemBody;
        bodyPreview: string;
        importance: Microsoft.Graph.importance;
        sensitivity: Microsoft.Graph.sensitivity;
        start: Microsoft.Graph.dateTimeTimeZone;
        originalStart: Date;
        end: Microsoft.Graph.dateTimeTimeZone;
        location: Microsoft.Graph.location;
        isAllDay: boolean;
        isCancelled: boolean;
        isOrganizer: boolean;
        recurrence: Microsoft.Graph.patternedRecurrence;
        responseRequested: boolean;
        seriesMasterId: string;
        showAs: Microsoft.Graph.freeBusyStatus;
        type: Microsoft.Graph.eventType;
        attendees: Microsoft.Graph.attendee[];
        organizer: Microsoft.Graph.recipient;
        webLink: string;
        calendar: Microsoft.Graph.calendar;
        instances: Microsoft.Graph.event[];
        extensions: Microsoft.Graph.extension[];
        attachments: Microsoft.Graph.attachment[];
    }
    interface responseStatus {
        response: Microsoft.Graph.responseType;
        time: Date;
    }
    interface dateTimeTimeZone {
        dateTime: string;
        timeZone: string;
    }
    interface location {
        displayName: string;
        locationEmailAddress: string;
        address: Microsoft.Graph.physicalAddress;
        coordinates: Microsoft.Graph.outlookGeoCoordinates;
    }
    interface physicalAddress {
        street: string;
        city: string;
        state: string;
        countryOrRegion: string;
        postalCode: string;
    }
    interface outlookGeoCoordinates {
        altitude: number;
        latitude: number;
        longitude: number;
        accuracy: number;
        altitudeAccuracy: number;
    }
    interface patternedRecurrence {
        pattern: Microsoft.Graph.recurrencePattern;
        range: Microsoft.Graph.recurrenceRange;
    }
    interface recurrencePattern {
        type: Microsoft.Graph.recurrencePatternType;
        interval: number;
        month: number;
        dayOfMonth: number;
        daysOfWeek: Microsoft.Graph.dayOfWeek[];
        firstDayOfWeek: Microsoft.Graph.dayOfWeek;
        index: Microsoft.Graph.weekIndex;
    }
    interface recurrenceRange {
        type: Microsoft.Graph.recurrenceRangeType;
        startDate: Microsoft.OData.Edm.Library.Date;
        endDate: Microsoft.OData.Edm.Library.Date;
        recurrenceTimeZone: string;
        numberOfOccurrences: number;
    }
    interface attendee extends Microsoft.Graph.attendeeBase {
        status: Microsoft.Graph.responseStatus;
    }
    interface attendeeBase extends Microsoft.Graph.recipient {
        type: Microsoft.Graph.attendeeType;
    }
    interface conversation extends Microsoft.OData.Client.BaseEntityType {
        topic: string;
        hasAttachments: boolean;
        lastDeliveredDateTime: Date;
        uniqueSenders: string[];
        preview: string;
        id: string;
        threads: Microsoft.Graph.conversationThread[];
    }
    interface profilePhoto extends Microsoft.OData.Client.BaseEntityType {
        height: number;
        width: number;
        id: string;
    }
    interface drive extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        driveType: string;
        owner: Microsoft.Graph.identitySet;
        quota: Microsoft.Graph.quota;
        items: Microsoft.Graph.driveItem[];
        special: Microsoft.Graph.driveItem[];
        root: Microsoft.Graph.driveItem;
    }
    interface identitySet {
        application: Microsoft.Graph.identity;
        device: Microsoft.Graph.identity;
        user: Microsoft.Graph.identity;
    }
    interface identity {
        displayName: string;
        id: string;
    }
    interface quota {
        deleted: number;
        remaining: number;
        state: string;
        total: number;
        used: number;
    }
    interface driveItem extends Microsoft.OData.Client.BaseEntityType {
        content: Microsoft.OData.Client.DataServiceStreamLink;
        createdBy: Microsoft.Graph.identitySet;
        createdDateTime: Date;
        cTag: string;
        description: string;
        eTag: string;
        id: string;
        lastModifiedBy: Microsoft.Graph.identitySet;
        lastModifiedDateTime: Date;
        name: string;
        parentReference: Microsoft.Graph.itemReference;
        size: number;
        webDavUrl: string;
        webUrl: string;
        audio: Microsoft.Graph.audio;
        deleted: Microsoft.Graph.deleted;
        file: Microsoft.Graph.file;
        fileSystemInfo: Microsoft.Graph.fileSystemInfo;
        folder: Microsoft.Graph.folder;
        image: Microsoft.Graph.image;
        location: Microsoft.Graph.geoCoordinates;
        photo: Microsoft.Graph.photo;
        remoteItem: Microsoft.Graph.remoteItem;
        searchResult: Microsoft.Graph.searchResult;
        shared: Microsoft.Graph.shared;
        specialFolder: Microsoft.Graph.specialFolder;
        video: Microsoft.Graph.video;
        createdByUser: Microsoft.Graph.user;
        lastModifiedByUser: Microsoft.Graph.user;
        permissions: Microsoft.Graph.permission[];
        children: Microsoft.Graph.driveItem[];
        thumbnails: Microsoft.Graph.thumbnailSet[];
    }
    interface itemReference {
        driveId: string;
        id: string;
        path: string;
    }
    interface audio {
        album: string;
        albumArtist: string;
        artist: string;
        bitrate: number;
        composers: string;
        copyright: string;
        disc: number;
        discCount: number;
        duration: number;
        genre: string;
        hasDrm: boolean;
        isVariableBitrate: boolean;
        title: string;
        track: number;
        trackCount: number;
        year: number;
    }
    interface deleted {
        state: string;
    }
    interface file {
        hashes: Microsoft.Graph.hashes;
        mimeType: string;
    }
    interface hashes {
        crc32Hash: string;
        sha1Hash: string;
    }
    interface fileSystemInfo {
        createdDateTime: Date;
        lastModifiedDateTime: Date;
    }
    interface folder {
        childCount: number;
    }
    interface image {
        height: number;
        width: number;
    }
    interface geoCoordinates {
        altitude: number;
        latitude: number;
        longitude: number;
    }
    interface photo {
        cameraMake: string;
        cameraModel: string;
        exposureDenominator: number;
        exposureNumerator: number;
        focalLength: number;
        fNumber: number;
        takenDateTime: Date;
        iso: number;
    }
    interface remoteItem {
        fileSystemInfo: Microsoft.Graph.fileSystemInfo;
        folder: Microsoft.Graph.folder;
        id: string;
        parentReference: Microsoft.Graph.itemReference;
        size: number;
    }
    interface searchResult {
        onClickTelemetryUrl: string;
    }
    interface shared {
        owner: Microsoft.Graph.identitySet;
        scope: string;
    }
    interface specialFolder {
        name: string;
    }
    interface video {
        bitrate: number;
        duration: number;
        height: number;
        width: number;
    }
    interface user extends Microsoft.Graph.directoryObject {
        accountEnabled: boolean;
        assignedLicenses: Microsoft.Graph.assignedLicense[];
        assignedPlans: Microsoft.Graph.assignedPlan[];
        businessPhones: string[];
        city: string;
        companyName: string;
        country: string;
        department: string;
        displayName: string;
        givenName: string;
        jobTitle: string;
        mail: string;
        mailNickname: string;
        mobilePhone: string;
        onPremisesImmutableId: string;
        onPremisesLastSyncDateTime: Date;
        onPremisesSecurityIdentifier: string;
        onPremisesSyncEnabled: boolean;
        passwordPolicies: string;
        passwordProfile: Microsoft.Graph.passwordProfile;
        officeLocation: string;
        postalCode: string;
        preferredLanguage: string;
        provisionedPlans: Microsoft.Graph.provisionedPlan[];
        proxyAddresses: string[];
        state: string;
        streetAddress: string;
        surname: string;
        usageLocation: string;
        userPrincipalName: string;
        userType: string;
        aboutMe: string;
        birthday: Date;
        hireDate: Date;
        interests: string[];
        mySite: string;
        pastProjects: string[];
        preferredName: string;
        responsibilities: string[];
        schools: string[];
        skills: string[];
        ownedDevices: Microsoft.Graph.directoryObject[];
        registeredDevices: Microsoft.Graph.directoryObject[];
        manager: Microsoft.Graph.directoryObject;
        directReports: Microsoft.Graph.directoryObject[];
        memberOf: Microsoft.Graph.directoryObject[];
        createdObjects: Microsoft.Graph.directoryObject[];
        ownedObjects: Microsoft.Graph.directoryObject[];
        messages: Microsoft.Graph.message[];
        joinedGroups: Microsoft.Graph.group[];
        mailFolders: Microsoft.Graph.mailFolder[];
        calendar: Microsoft.Graph.calendar;
        calendars: Microsoft.Graph.calendar[];
        calendarGroups: Microsoft.Graph.calendarGroup[];
        calendarView: Microsoft.Graph.event[];
        events: Microsoft.Graph.event[];
        people: Microsoft.Graph.person[];
        contacts: Microsoft.Graph.contact[];
        contactFolders: Microsoft.Graph.contactFolder[];
        inferenceClassification: Microsoft.Graph.inferenceClassification;
        photo: Microsoft.Graph.profilePhoto;
        photos: Microsoft.Graph.profilePhoto[];
        drive: Microsoft.Graph.drive;
        TrendingAround: Microsoft.Graph.driveItem[];
        WorkingWith: Microsoft.Graph.user[];
        notes: Microsoft.Graph.notes;
        tasks: Microsoft.Graph.task[];
        plans: Microsoft.Graph.plan[];
    }
    interface assignedLicense {
        disabledPlans: System.Guid[];
        skuId: System.Guid;
    }
    interface assignedPlan {
        assignedDateTime: Date;
        capabilityStatus: string;
        service: string;
        servicePlanId: System.Guid;
    }
    interface passwordProfile {
        password: string;
        forceChangePasswordNextSignIn: boolean;
    }
    interface provisionedPlan {
        capabilityStatus: string;
        provisioningStatus: string;
        service: string;
    }
    interface message extends Microsoft.Graph.outlookItem {
        receivedDateTime: Date;
        sentDateTime: Date;
        hasAttachments: boolean;
        subject: string;
        body: Microsoft.Graph.itemBody;
        bodyPreview: string;
        importance: Microsoft.Graph.importance;
        parentFolderId: string;
        sender: Microsoft.Graph.recipient;
        from: Microsoft.Graph.recipient;
        toRecipients: Microsoft.Graph.recipient[];
        ccRecipients: Microsoft.Graph.recipient[];
        bccRecipients: Microsoft.Graph.recipient[];
        replyTo: Microsoft.Graph.recipient[];
        conversationId: string;
        uniqueBody: Microsoft.Graph.itemBody;
        isDeliveryReceiptRequested: boolean;
        isReadReceiptRequested: boolean;
        isRead: boolean;
        isDraft: boolean;
        webLink: string;
        inferenceClassification: Microsoft.Graph.inferenceClassificationType;
        extensions: Microsoft.Graph.extension[];
        attachments: Microsoft.Graph.attachment[];
    }
    interface group extends Microsoft.Graph.directoryObject {
        description: string;
        displayName: string;
        groupTypes: string[];
        mail: string;
        mailEnabled: boolean;
        mailNickname: string;
        onPremisesLastSyncDateTime: Date;
        onPremisesSecurityIdentifier: string;
        onPremisesSyncEnabled: boolean;
        proxyAddresses: string[];
        securityEnabled: boolean;
        visibility: string;
        accessType: Microsoft.Graph.groupAccessType;
        allowExternalSenders: boolean;
        autoSubscribeNewMembers: boolean;
        isFavorite: boolean;
        isSubscribedByMail: boolean;
        unseenCount: number;
        members: Microsoft.Graph.directoryObject[];
        memberOf: Microsoft.Graph.directoryObject[];
        createdOnBehalfOf: Microsoft.Graph.directoryObject;
        owners: Microsoft.Graph.directoryObject[];
        threads: Microsoft.Graph.conversationThread[];
        calendar: Microsoft.Graph.calendar;
        calendarView: Microsoft.Graph.event[];
        events: Microsoft.Graph.event[];
        conversations: Microsoft.Graph.conversation[];
        photo: Microsoft.Graph.profilePhoto;
        photos: Microsoft.Graph.profilePhoto[];
        acceptedSenders: Microsoft.Graph.directoryObject[];
        rejectedSenders: Microsoft.Graph.directoryObject[];
        drive: Microsoft.Graph.drive;
        notes: Microsoft.Graph.notes;
        plans: Microsoft.Graph.plan[];
    }
    interface notes extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        notebooks: Microsoft.Graph.notebook[];
        sections: Microsoft.Graph.section[];
        sectionGroups: Microsoft.Graph.sectionGroup[];
        pages: Microsoft.Graph.page[];
        resources: Microsoft.Graph.resource[];
        operations: Microsoft.Graph.notesOperation[];
    }
    interface notebook extends Microsoft.OData.Client.BaseEntityType {
        isDefault: boolean;
        userRole: Microsoft.Graph.UserRole;
        isShared: boolean;
        sectionsUrl: string;
        sectionGroupsUrl: string;
        links: Microsoft.Graph.notebookLinks;
        name: string;
        createdBy: string;
        createdByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedBy: string;
        lastModifiedByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedTime: Date;
        id: string;
        self: string;
        createdTime: Date;
        sections: Microsoft.Graph.section[];
        sectionGroups: Microsoft.Graph.sectionGroup[];
    }
    interface notebookLinks {
        oneNoteClientUrl: Microsoft.Graph.externalLink;
        oneNoteWebUrl: Microsoft.Graph.externalLink;
    }
    interface externalLink {
        href: string;
    }
    interface oneNoteIdentitySet {
        user: Microsoft.Graph.oneNoteIdentity;
    }
    interface oneNoteIdentity {
        id: string;
        displayName: string;
    }
    interface section extends Microsoft.OData.Client.BaseEntityType {
        isDefault: boolean;
        pagesUrl: string;
        name: string;
        createdBy: string;
        createdByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedBy: string;
        lastModifiedByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedTime: Date;
        id: string;
        self: string;
        createdTime: Date;
        parentNotebook: Microsoft.Graph.notebook;
        parentSectionGroup: Microsoft.Graph.sectionGroup;
        pages: Microsoft.Graph.page[];
    }
    interface sectionGroup extends Microsoft.OData.Client.BaseEntityType {
        sectionsUrl: string;
        sectionGroupsUrl: string;
        name: string;
        createdBy: string;
        createdByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedBy: string;
        lastModifiedByIdentity: Microsoft.Graph.oneNoteIdentitySet;
        lastModifiedTime: Date;
        id: string;
        self: string;
        createdTime: Date;
        parentNotebook: Microsoft.Graph.notebook;
        parentSectionGroup: Microsoft.Graph.sectionGroup;
        sections: Microsoft.Graph.section[];
        sectionGroups: Microsoft.Graph.sectionGroup[];
    }
    interface page extends Microsoft.OData.Client.BaseEntityType {
        title: string;
        createdByAppId: string;
        links: Microsoft.Graph.pageLinks;
        contentUrl: string;
        content: Microsoft.OData.Client.DataServiceStreamLink;
        lastModifiedTime: Date;
        level: number;
        order: number;
        id: string;
        self: string;
        createdTime: Date;
        parentSection: Microsoft.Graph.section;
        parentNotebook: Microsoft.Graph.notebook;
    }
    interface pageLinks {
        oneNoteClientUrl: Microsoft.Graph.externalLink;
        oneNoteWebUrl: Microsoft.Graph.externalLink;
    }
    interface resource extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        self: string;
        content: Microsoft.OData.Client.DataServiceStreamLink;
        contentUrl: string;
    }
    interface notesOperation extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        status: string;
        createdDateTime: Date;
        lastActionDateTime: Date;
        resourceLocation: string;
        resourceId: string;
        error: Microsoft.Graph.notesOperationError;
    }
    interface notesOperationError {
        code: string;
        message: string;
    }
    interface plan extends Microsoft.OData.Client.BaseEntityType {
        createdBy: string;
        owner: string;
        title: string;
        id: string;
        tasks: Microsoft.Graph.task[];
        buckets: Microsoft.Graph.bucket[];
        details: Microsoft.Graph.planDetails;
        assignedToTaskBoard: Microsoft.Graph.planTaskBoard;
        progressTaskBoard: Microsoft.Graph.planTaskBoard;
        bucketTaskBoard: Microsoft.Graph.planTaskBoard;
    }
    interface task extends Microsoft.OData.Client.BaseEntityType {
        createdBy: string;
        assignedTo: string;
        planId: string;
        bucketId: string;
        title: string;
        orderHint: string;
        assigneePriority: string;
        percentComplete: number;
        startDateTime: Date;
        assignedDateTime: Date;
        createdDateTime: Date;
        assignedBy: string;
        dueDateTime: Date;
        hasDescription: boolean;
        previewType: Microsoft.Graph.previewType;
        completedDateTime: Date;
        appliedCategories: Microsoft.Graph.appliedCategoriesCollection;
        conversationThreadId: string;
        id: string;
        details: Microsoft.Graph.taskDetails;
        assignedToTaskBoardFormat: Microsoft.Graph.taskBoardTaskFormat;
        progressTaskBoardFormat: Microsoft.Graph.taskBoardTaskFormat;
        bucketTaskBoardFormat: Microsoft.Graph.taskBoardTaskFormat;
    }
    interface appliedCategoriesCollection {
    }
    interface taskDetails extends Microsoft.OData.Client.BaseEntityType {
        description: string;
        previewType: Microsoft.Graph.previewType;
        completedBy: string;
        references: Microsoft.Graph.externalReferenceCollection;
        checklist: Microsoft.Graph.checklistItemCollection;
        id: string;
    }
    interface externalReferenceCollection {
    }
    interface checklistItemCollection {
    }
    interface taskBoardTaskFormat extends Microsoft.OData.Client.BaseEntityType {
        type: Microsoft.Graph.taskBoardType;
        orderHint: string;
        id: string;
    }
    interface bucket extends Microsoft.OData.Client.BaseEntityType {
        name: string;
        planId: string;
        orderHint: string;
        id: string;
        tasks: Microsoft.Graph.task[];
    }
    interface planDetails extends Microsoft.OData.Client.BaseEntityType {
        sharedWith: Microsoft.Graph.userIdCollection;
        category0Description: string;
        category1Description: string;
        category2Description: string;
        category3Description: string;
        category4Description: string;
        category5Description: string;
        id: string;
    }
    interface userIdCollection {
    }
    interface planTaskBoard extends Microsoft.OData.Client.BaseEntityType {
        type: Microsoft.Graph.taskBoardType;
        id: string;
    }
    interface mailFolder extends Microsoft.OData.Client.BaseEntityType {
        displayName: string;
        parentFolderId: string;
        childFolderCount: number;
        unreadItemCount: number;
        totalItemCount: number;
        id: string;
        messages: Microsoft.Graph.message[];
        childFolders: Microsoft.Graph.mailFolder[];
    }
    interface calendarGroup extends Microsoft.OData.Client.BaseEntityType {
        name: string;
        classId: System.Guid;
        changeKey: string;
        id: string;
        calendars: Microsoft.Graph.calendar[];
    }
    interface person extends Microsoft.OData.Client.BaseEntityType {
        sources: Microsoft.Graph.personDataSource[];
        displayName: string;
        givenName: string;
        surname: string;
        title: string;
        emailAddresses: Microsoft.Graph.email[];
        companyName: string;
        officeLocation: string;
        id: string;
    }
    interface personDataSource {
        type: string;
    }
    interface email {
        address: string;
    }
    interface contact extends Microsoft.Graph.outlookItem {
        parentFolderId: string;
        birthday: Date;
        fileAs: string;
        displayName: string;
        givenName: string;
        initials: string;
        middleName: string;
        nickName: string;
        surname: string;
        title: string;
        yomiGivenName: string;
        yomiSurname: string;
        yomiCompanyName: string;
        generation: string;
        emailAddresses: Microsoft.Graph.emailAddress[];
        imAddresses: string[];
        jobTitle: string;
        companyName: string;
        department: string;
        officeLocation: string;
        profession: string;
        businessHomePage: string;
        assistantName: string;
        manager: string;
        homePhones: string[];
        mobilePhone1: string;
        businessPhones: string[];
        homeAddress: Microsoft.Graph.physicalAddress;
        businessAddress: Microsoft.Graph.physicalAddress;
        otherAddress: Microsoft.Graph.physicalAddress;
        spouseName: string;
        personalNotes: string;
        children: string[];
        extensions: Microsoft.Graph.extension[];
        photo: Microsoft.Graph.profilePhoto;
    }
    interface contactFolder extends Microsoft.OData.Client.BaseEntityType {
        parentFolderId: string;
        displayName: string;
        id: string;
        contacts: Microsoft.Graph.contact[];
        childFolders: Microsoft.Graph.contactFolder[];
    }
    interface inferenceClassification extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        overrides: Microsoft.Graph.inferenceClassificationOverride[];
    }
    interface inferenceClassificationOverride extends Microsoft.OData.Client.BaseEntityType {
        classifyAs: Microsoft.Graph.inferenceClassificationType;
        senderEmailAddress: Microsoft.Graph.emailAddress;
        id: string;
    }
    interface permission extends Microsoft.OData.Client.BaseEntityType {
        grantedTo: Microsoft.Graph.identitySet;
        id: string;
        invitation: Microsoft.Graph.sharingInvitation;
        inheritedFrom: Microsoft.Graph.itemReference;
        link: Microsoft.Graph.sharingLink;
        roles: string[];
        shareId: string;
    }
    interface sharingInvitation {
        email: string;
        redeemedBy: string;
        signInRequired: boolean;
    }
    interface sharingLink {
        application: Microsoft.Graph.identity;
        type: string;
        webUrl: string;
    }
    interface thumbnailSet extends Microsoft.OData.Client.BaseEntityType {
        id: string;
        large: Microsoft.Graph.thumbnail;
        medium: Microsoft.Graph.thumbnail;
        small: Microsoft.Graph.thumbnail;
        source: Microsoft.Graph.thumbnail;
    }
    interface thumbnail {
        content: Microsoft.OData.Client.DataServiceStreamLink;
        height: number;
        url: string;
        width: number;
    }
    interface oAuth2PermissionGrant extends Microsoft.OData.Client.BaseEntityType {
        clientId: string;
        consentType: string;
        expiryTime: Date;
        id: string;
        principalId: string;
        resourceId: string;
        scope: string;
        startTime: Date;
    }
    interface subscribedSku extends Microsoft.OData.Client.BaseEntityType {
        capabilityStatus: string;
        consumedUnits: number;
        id: string;
        prepaidUnits: Microsoft.Graph.licenseUnitsDetail;
        servicePlans: Microsoft.Graph.servicePlanInfo[];
        skuId: System.Guid;
        skuPartNumber: string;
        appliesTo: string;
    }
    interface licenseUnitsDetail {
        enabled: number;
        suspended: number;
        warning: number;
    }
    interface servicePlanInfo {
        servicePlanId: System.Guid;
        servicePlanName: string;
        provisioningStatus: string;
        appliesTo: string;
    }
    interface subscription extends Microsoft.OData.Client.BaseEntityType {
        subscriptionId: string;
        resource: string;
        changeType: string;
        clientState: string;
        notificationUrl: string;
        subscriptionExpirationDateTime: Date;
    }
}
declare module Microsoft.OData.Client {
    interface BaseEntityType {
    }
    interface DataServiceStreamLink {
        Name: string;
        SelfLink: System.Uri;
        EditLink: System.Uri;
        ContentType: string;
        ETag: string;
    }
}
declare module System {
    interface Guid {
    }
    interface Uri {
        AbsolutePath: string;
        AbsoluteUri: string;
        LocalPath: string;
        Authority: string;
        HostNameType: System.UriHostNameType;
        IsDefaultPort: boolean;
        IsFile: boolean;
        IsLoopback: boolean;
        PathAndQuery: string;
        Segments: string[];
        IsUnc: boolean;
        Host: string;
        Port: number;
        Query: string;
        Fragment: string;
        Scheme: string;
        OriginalString: string;
        DnsSafeHost: string;
        IdnHost: string;
        IsAbsoluteUri: boolean;
        UserEscaped: boolean;
        UserInfo: string;
    }
}
declare module Microsoft.OData.Edm.Library {
    interface Date {
        Now: Microsoft.OData.Edm.Library.Date;
        Year: number;
        Month: number;
        Day: number;
    }
}
module Microsoft.Graph {
    export const enum bodyType {
        text = 0,
        html = 1
    }
    export const enum responseType {
        none = 0,
        organizer = 1,
        tentativelyAccepted = 2,
        accepted = 3,
        declined = 4,
        notResponded = 5
    }
    export const enum dayOfWeek {
        sunday = 0,
        monday = 1,
        tuesday = 2,
        wednesday = 3,
        thursday = 4,
        friday = 5,
        saturday = 6
    }
    export const enum recurrencePatternType {
        daily = 0,
        weekly = 1,
        absoluteMonthly = 2,
        relativeMonthly = 3,
        absoluteYearly = 4,
        relativeYearly = 5
    }
    export const enum weekIndex {
        first = 0,
        second = 1,
        third = 2,
        fourth = 3,
        last = 4
    }
    export const enum recurrenceRangeType {
        endDate = 0,
        noEnd = 1,
        numbered = 2
    }
    export const enum attendeeType {
        required = 0,
        optional = 1,
        resource = 2
    }
    export const enum importance {
        low = 0,
        normal = 1,
        high = 2
    }
    export const enum sensitivity {
        normal = 0,
        personal = 1,
        private = 2,
        confidential = 3
    }
    export const enum freeBusyStatus {
        free = 0,
        tentative = 1,
        busy = 2,
        oof = 3,
        workingElsewhere = 4,
        unknown = -1
    }
    export const enum eventType {
        singleInstance = 0,
        occurrence = 1,
        exception = 2,
        seriesMaster = 3
    }
    export const enum calendarColor {
        lightBlue = 0,
        lightGreen = 1,
        lightOrange = 2,
        lightGray = 3,
        lightYellow = 4,
        lightTeal = 5,
        lightPink = 6,
        lightBrown = 7,
        lightRed = 8,
        maxColor = 9,
        auto = -1
    }
    export const enum inferenceClassificationType {
        focused = 0,
        other = 1
    }
    export const enum UserRole {
        Owner = 0,
        Contributor = 1,
        Reader = 2,
        None = -1
    }
    export const enum previewType {
        automatic = 0,
        noPreview = 1,
        checklist = 2,
        description = 3,
        reference = 4
    }
    export const enum taskBoardType {
        progress = 0,
        assignedTo = 1,
        bucket = 2
    }
    export const enum groupAccessType {
        none = 0,
        private = 1,
        secret = 2,
        public = 3
    }
}
module System {
    export const enum UriHostNameType {
        Unknown = 0,
        Basic = 1,
        Dns = 2,
        IPv4 = 3,
        IPv6 = 4
    }
}
