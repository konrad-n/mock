export var ModuleType;
(function (ModuleType) {
    ModuleType["Basic"] = "Basic";
    ModuleType["Specialist"] = "Specialist";
})(ModuleType || (ModuleType = {}));
export const getModuleLabel = (type) => {
    return type === ModuleType.Basic ? 'Moduł Podstawowy' : 'Moduł Specjalistyczny';
};
//# sourceMappingURL=ModuleType.js.map