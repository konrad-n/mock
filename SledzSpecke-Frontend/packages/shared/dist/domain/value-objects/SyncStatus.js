export var SyncStatus;
(function (SyncStatus) {
    SyncStatus["NotSynced"] = "NotSynced";
    SyncStatus["Synced"] = "Synced";
    SyncStatus["Modified"] = "Modified";
    SyncStatus["Approved"] = "Approved";
})(SyncStatus || (SyncStatus = {}));
export const isSyncStatusEditable = (status) => {
    return status !== SyncStatus.Approved;
};
//# sourceMappingURL=SyncStatus.js.map