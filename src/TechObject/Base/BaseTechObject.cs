﻿using EasyEPlanner;
using System.Collections.Generic;
using System.Linq;

namespace TechObject
{
    /// <summary>
    /// Класс реализующий базовый аппарат для технологического объекта
    /// </summary>
    public class BaseTechObject
    {
        public BaseTechObject(TechObject owner = null)
        {
            Name = string.Empty;
            EplanName = string.Empty;
            S88Level = 0;
            BaseOperations = new List<BaseOperation>();
            BasicName = string.Empty;
            Owner = owner;
            Equipment = new List<BaseParameter>();
            AggregateParameters = new List<BaseParameter>();
            BindingName = string.Empty;
            SystemParams = new SystemParams();

            objectGroups = new List<AttachedObjects>();
        }

        /// <summary>
        /// Добавить оборудование в базовый объект
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="value">Значение</param>
        public void AddEquipment(string luaName, string name, string value)
        {
            var equipment = new EquipmentParameter(luaName, name, value);
            equipment.Owner = this;
            Equipment.Add(equipment);
        }


        /// <summary>
        /// Добавить активный параметр агрегата
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        /// <returns>Добавленный параметр</returns>
        public ActiveParameter AddActiveParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveParameter(luaName, name, defaultValue);
            par.Owner = this;
            AggregateParameters.Add(par);
            return par;
        }

        /// <summary>
        /// Добавить активный булевый параметр агрегата
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultValue">Значение по-умолчанию</param>
        public void AddActiveBoolParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new ActiveBoolParameter(luaName, name, defaultValue);
            par.Owner = this;
            AggregateParameters.Add(par);
        }

        /// <summary>
        /// Добавить главный параметр агрегата
        /// </summary>
        /// <param name="luaName"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public void AddMainAggregateParameter(string luaName, string name,
            string defaultValue)
        {
            var par = new MainAggregateParameter(luaName, name, defaultValue);
            par.Owner = this;
            aggregateMainParameter = par;
        }

        /// <summary>
        /// Добавить базовую операцию
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <param name="name">Имя</param>
        /// <param name="defaultPosition">Стандартная позиция операции при
        /// автоматической настройке</param>
        /// <returns></returns>
        public BaseOperation AddBaseOperation(string luaName, string name,
            int defaultPosition)
        {
            if (BaseOperations.Count == 0)
            {
                // Пустой объект, если не должно быть выбрано никаких объектов
                BaseOperations.Add(BaseOperation.EmptyOperation());
            }

            var operation = BaseOperation.EmptyOperation();
            operation.LuaName = luaName;
            operation.Name = name;
            operation.DefaultPosition = defaultPosition;
            BaseOperations.Add(operation);

            return operation;
        }

        /// <summary>
        /// Добавить группу объектов в объект
        /// </summary>
        /// <param name="luaName">Lua-имя группы</param>
        /// <param name="name">Отображаемое имя группы</param>
        /// <param name="allowedObjects">Разрешенные типы объектов для
        /// добавления в группу</param>
        public void AddObjectGroup(string luaName, string name,
            string allowedObjects)
        {
            AttachedObjects newGroup = MakeObjectGroup(luaName, name,
                allowedObjects);
            if (newGroup != null)
            {
                objectGroups.Add(newGroup);
            }
        }

        public void AddSystemParameter(string luaName, string name,
            double value, string meter)
        {
            var param = new SystemParam(systemParams.GetIdx, name, value,
                meter, luaName);
            systemParams.AddParam(param);
        }

        /// <summary>
        /// Создать объект группы объектов.
        /// </summary>
        /// <param name="luaName">Lua-имя группы</param>
        /// <param name="name">Отображаемое имя группы</param>
        /// <param name="allowedObjects">Разрешенные типы объектов
        /// для добавления в группу</param>
        /// <returns></returns>
        private AttachedObjects MakeObjectGroup(string luaName, string name,
            string allowedObjects)
        {
            List<BaseTechObjectManager.ObjectType> allowedObjectsList;
            switch (allowedObjects)
            {
                case "all":
                    allowedObjectsList =
                        new List<BaseTechObjectManager.ObjectType>()
                        {
                            BaseTechObjectManager.ObjectType.Aggregate,
                            BaseTechObjectManager.ObjectType.Unit
                        };

                    return new AttachedObjects(string.Empty, null,
                        new AttachedObjectStrategy.AttachedWithoutInitStrategy(
                            name, luaName, allowedObjectsList));

                case "units":
                    allowedObjectsList =
                        new List<BaseTechObjectManager.ObjectType>()
                        {
                            BaseTechObjectManager.ObjectType.Unit
                        };

                    return new AttachedObjects(string.Empty, null,
                        new AttachedObjectStrategy.AttachedWithoutInitStrategy(
                            name, luaName, allowedObjectsList));

                case "aggregates":
                    allowedObjectsList =
                        new List<BaseTechObjectManager.ObjectType>()
                        {
                            BaseTechObjectManager.ObjectType.Aggregate,
                        };

                    return new AttachedObjects(string.Empty, null,
                        new AttachedObjectStrategy.AttachedWithoutInitStrategy(
                            name, luaName, allowedObjectsList));

                default:
                    // Default value - aggregate.
                    allowedObjectsList =
                        new List<BaseTechObjectManager.ObjectType>()
                        {
                            BaseTechObjectManager.ObjectType.Aggregate,
                        };
                    return new AttachedObjects(string.Empty, null,
                        new AttachedObjectStrategy.AttachedWithoutInitStrategy(
                            name, luaName, allowedObjectsList));
            }
        }

        /// <summary>
        /// Имя базового объекта.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// ОУ базового объекта
        /// </summary>
        public string EplanName
        {
            get
            {
                return eplanName;
            }

            set
            {
                eplanName = value;
            }
        }

        /// <summary>
        /// Уровень по S88 иерархии
        /// </summary>
        public int S88Level
        {
            get
            {
                return s88Level;
            }

            set
            {
                s88Level = value;
            }
        }

        /// <summary>
        /// Базовые операции объекта
        /// </summary>
        public List<BaseOperation> BaseOperations
        {
            get
            {
                return objectOperations;
            }

            set
            {
                objectOperations = value;
            }
        }

        /// <summary>
        /// Имя объекта для базовой функциональности
        /// </summary>
        public string BasicName
        {
            get
            {
                return basicName;
            }

            set
            {
                basicName = value;
            }
        }

        /// <summary>
        /// Владелец объекта.
        /// </summary>
        public TechObject Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Оборудование базового объекта
        /// </summary>
        public List<BaseParameter> Equipment
        {
            get
            {
                return equipment;
            }

            set
            {
                equipment = value;
            }
        }

        /// <summary>
        /// Получить базовую операцию по имени.
        /// </summary>
        /// <param name="name">Имя</param>
        /// <returns></returns>
        public BaseOperation GetBaseOperationByName(string name)
        {
            var operation = BaseOperations.Where(x => x.Name == name)
                .FirstOrDefault();
            return operation;
        }

        /// <summary>
        /// Получить базовую операцию по Lua-имени
        /// </summary>
        /// <param name="luaName">Lua-имя</param>
        /// <returns></returns>
        public BaseOperation GetBaseOperationByLuaName(string luaName)
        {
            var operation = BaseOperations.Where(x => x.LuaName == luaName)
                .FirstOrDefault();
            return operation;
        }

        /// <summary>
        /// Список операций базового объекта
        /// </summary>
        /// <returns></returns>
        public List<string> BaseOperationsList
        {
            get
            {
                return BaseOperations.Select(x => x.Name).ToList();
            }
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <param name="techObject">Копируемый объект</param>
        /// <returns></returns>
        public BaseTechObject Clone(TechObject techObject)
        {
            var cloned = Clone();
            cloned.Owner = techObject;
            foreach(var clonedObjectGroup in cloned.objectGroups)
            {
                clonedObjectGroup.Owner = techObject;
            }
            return cloned;
        }

        /// <summary>
        /// Копия объекта
        /// </summary>
        /// <returns></returns>
        public BaseTechObject Clone()
        {
            var cloned = new BaseTechObject(Owner);
            cloned.Name = Name;

            var aggregateParameters = new List<BaseParameter>();
            foreach (var aggrPar in AggregateParameters)
            {
                aggregateParameters.Add(aggrPar.Clone());
            }
            cloned.AggregateParameters = aggregateParameters;
            if (MainAggregateParameter != null)
            {
                cloned.MainAggregateParameter = MainAggregateParameter.Clone()
                    as MainAggregateParameter;
            }

            var baseOperations = new List<BaseOperation>();
            foreach (var baseOperation in BaseOperations)
            {
                baseOperations.Add(baseOperation.Clone());
            }
            cloned.BaseOperations = baseOperations;

            cloned.BasicName = BasicName;
            cloned.EplanName = EplanName;

            var equipment = new List<BaseParameter>();
            foreach (var equip in Equipment)
            {
                var newEquip = equip.Clone();
                newEquip.Owner = this;
                equipment.Add(newEquip);
            }
            cloned.Equipment = equipment;

            cloned.S88Level = S88Level;
            cloned.BindingName = BindingName;
            cloned.IsPID = IsPID;

            cloned.objectGroups = new List<AttachedObjects>();
            foreach(var objectGroup in objectGroups)
            {
                var clonedStrategy = new AttachedObjectStrategy
                    .AttachedWithoutInitStrategy(
                    objectGroup.WorkStrategy.Name,
                    objectGroup.WorkStrategy.LuaName,
                    objectGroup.WorkStrategy.AllowedObjects);
                var clonedGroup = new AttachedObjects(objectGroup.Value,
                    objectGroup.Owner, clonedStrategy);
                cloned.objectGroups.Add(clonedGroup);
            }

            cloned.SystemParams = systemParams.Clone();

            return cloned;
        }

        /// <summary>
        /// Является ли базовый объект привязываемым к другому объекту.
        /// </summary>
        public virtual bool IsAttachable
        {
            get
            {
                bool isAttachable = UseGroups ||
                    S88Level == (int)BaseTechObjectManager.ObjectType.Unit ||
                    S88Level == (int)BaseTechObjectManager.ObjectType.Aggregate;
                return isAttachable;
            }
        }

        /// <summary>
        /// Параметры объекта, как агрегата (добавляемые в аппарат).
        /// </summary>
        public List<BaseParameter> AggregateParameters
        {
            get
            {
                if (aggregateProperties == null)
                {
                    return new List<BaseParameter>();
                }
                else
                {
                    return aggregateProperties;
                }
            }

            set
            {
                aggregateProperties = value;
            }
        }

        /// <summary>
        /// Имя агрегата при его привязке к аппарату.
        /// </summary>
        public string BindingName
        {
            get
            {
                return bindingName;
            }

            set
            {
                bindingName = value;
            }
        }

        /// <summary>
        /// Главный параметр агрегата
        /// </summary>
        public MainAggregateParameter MainAggregateParameter
        {
            get
            {
                return aggregateMainParameter;
            }
            set
            {
                aggregateMainParameter = value;
            }
        }

        /// <summary>
        /// Является ли объект ПИД-регулятором
        /// </summary>
        public bool IsPID { get; set; } = default;

        /// <summary>
        /// Использовать ли группы объектов
        /// </summary>
        public bool UseGroups
        {
            get
            {
                return ObjectGroupsList.Count > 0;
            }
        }

        /// <summary>
        /// Группа объектов
        /// </summary>
        public List<AttachedObjects> ObjectGroupsList 
        {
            get => objectGroups; 
        }

        public SystemParams SystemParams
        {
            get
            {
                return systemParams;
            }
            set 
            {
                systemParams = value;
            }
        }

        #region Сохранение в prg.lua
        /// <summary>
        /// Сохранить информацию об объекте в prg.lua
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        public string SaveObjectInfoToPrgLua(string objName, string prefix)
        {
            var res = string.Empty;

            res += SaveTankAdditionalParameters(objName, prefix);
            res += SaveLineAdditionalParameters(objName, prefix);

            return res;
        }

        /// <summary>
        /// Сохранить доп. информацию о танке
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveTankAdditionalParameters(string objName,
            string prefix)
        {
            var res = string.Empty;
            if (S88Level == (int)BaseTechObjectManager.ObjectType.Unit)
            {
                var masterObj = TechObjectManager.GetInstance()
                    .ProcessCellObject;
                if (masterObj != null)
                {
                    res += objName + ".master = prg." + masterObj.NameEplan
                        .ToLower() + masterObj.TechNumber + "\n";
                }

                // Параметры сбрасываемые до мойки.
                res += objName + ".reset_before_wash =\n" +
                    prefix + "{\n" +
                    prefix + objName + ".PAR_FLOAT.V_ACCEPTING_CURRENT,\n" +
                    prefix + objName + ".PAR_FLOAT.PRODUCT_TYPE,\n" +
                    prefix + objName + ".PAR_FLOAT.V_ACCEPTING_SET\n" +
                    prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить доп. информацию о линии
        /// </summary>
        /// <param name="objName">Имя объекта</param>
        /// <param name="prefix">Отступ</param>
        /// <returns></returns>
        private string SaveLineAdditionalParameters(string objName,
            string prefix)
        {
            var res = string.Empty;

            if (UseGroups && objectGroups.Count > 0)
            {
                foreach (var objectGroup in objectGroups)
                {
                    if (objectGroup.Value == string.Empty)
                    {
                        continue;
                    }

                    string objectNames = objectGroup.GetAttachedObjectsName()
                        .Select(x => $"{prefix}prg.{x},\n")
                        .Aggregate((x, y) => x + y);
                    res += $"{objName}.{objectGroup.WorkStrategy.LuaName} =\n";
                    res += $"{prefix}{{\n";
                    res += $"{objectNames}";
                    res += $"{prefix}}}\n";
                }
            }

            bool containsFillOrPumping = BaseOperations
                .Any(x => x.LuaName == "FILL" || x.LuaName == "PUMPING");
            if (UseGroups && containsFillOrPumping)
            {
                res += $"{objName}.reset_before_wash =\n" +
                    prefix + "{\n" +
                    prefix + $"{objName}.PAR_FLOAT.PROD_V,\n" +
                    prefix + $"{objName}.PAR_FLOAT.WATER_V,\n" +
                    prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить операции объекта
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        public string SaveOperations(string objName, string prefix, 
            List<Mode> modes)
        {
            var res = "";

            string saveOperations = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name != "")
                {
                    saveOperations += prefix + baseOperation.LuaName.ToUpper() +
                        " = " + mode.GetModeNumber() + ",\n";
                }
            }

            bool isEmpty = saveOperations == "";
            if (!isEmpty)
            {
                res += objName + ".operations = \t\t--Операции.\n";
                res += prefix + "{\n";
                res += saveOperations;
                res += prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить номера шагов операций объекта.
        /// </summary>
        /// <param name="objName">Имя объекта для записи</param>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        public string SaveOperationsSteps(string objName, string prefix,
            List<Mode> modes)
        {
            var res = "";

            string steps = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Name == "")
                {
                    continue;
                }

                string stepString = "";
                foreach (var step in mode.MainSteps)
                {
                    if (step.GetBaseStepName() == "")
                    {
                        continue;
                    }
                    stepString += prefix + prefix + step.GetBaseStepLuaName() +
                        " = " + step.GetStepNumber() + ",\n";
                }

                bool stepIsEmpty = stepString == "";
                if (!stepIsEmpty)
                {
                    steps += prefix + baseOperation.LuaName.ToUpper() + " =\n";
                    steps += prefix + prefix + "{\n";
                    steps += stepString;
                    steps += prefix + prefix + "},\n";
                }
            }

            bool stepsIsEmpty = steps == "";
            if(!stepsIsEmpty)
            {
                res += objName + ".steps = \t\t--Шаги операций.\n";
                res += prefix + "{\n";
                res += steps;
                res += prefix + "}\n";
            }

            return res;
        }

        /// <summary>
        /// Сохранить параметры операций базового объекта танк.
        /// </summary>
        /// <param name="prefix">Отступ</param>
        /// <param name="modes">Операции объекта</param>
        /// <returns></returns>
        public string SaveOperationsParameters(string objName,
            string prefix, List<Mode> modes)
        {
            var res = "";
            foreach (Mode mode in modes)
            {
                var baseOperation = mode.BaseOperation;
                if (baseOperation.Properties.Count == 0)
                {
                    continue;
                }

                string paramsForSave = "";
                foreach (var parameter in baseOperation.Properties)
                {
                    bool isEmpty = parameter.IsEmpty || 
                        parameter.Value == "" ||
                        parameter.NeedDisable;
                    if (isEmpty)
                    {
                        continue;
                    }

                    string paramCode = parameter.SaveToPrgLua(prefix);
                    if (!string.IsNullOrEmpty(paramCode))
                    {
                        paramsForSave += $"{paramCode},\n";
                    }
                }

                bool needSaveParameters = paramsForSave != "";
                if (needSaveParameters)
                {
                    res += $"{objName}.{baseOperation.LuaName} =\n";
                    res += prefix + "{\n";
                    res += paramsForSave;
                    res += prefix + "}\n";
                }
            }

            return res;
        }

        /// <summary>
        /// Сохранить оборудование технологического объекта
        /// </summary>
        /// <param name="objName">Имя для сохранения</param>
        /// <param name="obj">Объект с параметрами</param>
        /// <returns></returns>
        public string SaveEquipment(TechObject obj, string objName)
        {
            var res = "";
            Equipment equipment = obj.Equipment;
            foreach (var item in equipment.Items)
            {
                var property = item as BaseParameter;
                
                string equipmentCode = property.SaveToPrgLua(string.Empty);
                if (string.IsNullOrEmpty(equipmentCode))
                {
                    continue;
                }

                res += $"{objName}.{equipmentCode}\n";
            }

            return res;
        }
        #endregion

        private string name;
        private string eplanName;
        private int s88Level;
        private string basicName;
        private TechObject owner;
        private string bindingName;

        private List<BaseOperation> objectOperations;
        private List<BaseParameter> equipment;
        private List<BaseParameter> aggregateProperties;
        private MainAggregateParameter aggregateMainParameter;
        private List<AttachedObjects> objectGroups;
        private SystemParams systemParams;
    }
}
