export var SmkVersion;
(function (SmkVersion) {
    SmkVersion["Old"] = "old";
    SmkVersion["New"] = "new";
})(SmkVersion || (SmkVersion = {}));
export const getSmkVersionLabel = (version) => {
    return version === SmkVersion.Old ? 'Stary SMK' : 'Nowy SMK';
};
//# sourceMappingURL=SmkVersion.js.map