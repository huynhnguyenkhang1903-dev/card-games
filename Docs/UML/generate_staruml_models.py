import json, uuid

def gen_id():
    return uuid.uuid4().hex[:20]

def create_model_element(elem_type, name, parent_id, extra=None):
    elem = {
        "_type": elem_type,
        "_id": gen_id(),
        "_parent": {"$ref": parent_id},
        "name": name
    }
    if extra:
        elem.update(extra)
    return elem

# Project root
proj_id = gen_id()
model_id = gen_id()

project = {
    "_type": "Project",
    "_id": proj_id,
    "name": "Antigravity Game Project",
    "author": "Antigravity Scrum Team",
    "version": "1.0",
    "ownedElements": []
}

uml_model = {
    "_type": "UMLModel",
    "_id": model_id,
    "_parent": {"$ref": proj_id},
    "name": "Antigravity Core Model",
    "ownedElements": []
}
project["ownedElements"].append(uml_model)

# ----------------- US-005: Class Diagram & Model -----------------
class_diag_id = gen_id()
class_diagram = {
    "_type": "UMLClassDiagram",
    "_id": class_diag_id,
    "_parent": {"$ref": model_id},
    "name": "US-005_Class_Diagram_Card_Character",
    "defaultDiagram": True,
    "ownedViews": []
}
uml_model["ownedElements"].append(class_diagram)

# Enums
enums_data = [
    ("CardType", ["BasicAttack", "Skill", "Ultimate", "Tactical"]),
    ("TargetRange", ["SingleFrontline", "PierceColumn", "BlastRow", "AnyAlly", "AllAllies", "Self"]),
    ("GridRow", ["Frontline = 0", "Backline = 1"])
]

enum_refs = {}
for ename, literals in enums_data:
    eid = gen_id()
    enum_refs[ename] = eid
    l_objs = [{"_type": "UMLEnumerationLiteral", "_id": gen_id(), "_parent": {"$ref": eid}, "name": lit} for lit in literals]
    enum_elem = {
        "_type": "UMLEnumeration",
        "_id": eid,
        "_parent": {"$ref": model_id},
        "name": ename,
        "literals": l_objs
    }
    uml_model["ownedElements"].append(enum_elem)

# Interfaces
iface_data = [
    ("IDamageable", [
        ("CurrentHP", "int"),
        ("CurrentShield", "int")
    ], [
        ("TakeDamage", "void", [("amount", "int")]),
        ("AddShield", "void", [("amount", "int")]),
        ("Heal", "void", [("amount", "int")]),
        ("Die", "void", [])
    ]),
    ("IStatusEffect", [
        ("EffectName", "string"),
        ("Duration", "int"),
        ("StackCount", "int")
    ], [
        ("ApplyEffect", "void", [("target", "GameObject")]),
        ("RemoveEffect", "void", [("target", "GameObject")]),
        ("OnTurnStart", "void", []),
        ("OnTurnEnd", "void", [])
    ])
]

iface_refs = {}
for iname, attrs, ops in iface_data:
    iid = gen_id()
    iface_refs[iname] = iid
    attr_objs = [{"_type": "UMLAttribute", "_id": gen_id(), "_parent": {"$ref": iid}, "name": a[0], "type": a[1], "visibility": "public"} for a in attrs]
    op_objs = []
    for op in ops:
        op_id = gen_id()
        params = [{"_type": "UMLParameter", "_id": gen_id(), "_parent": {"$ref": op_id}, "name": p[0], "type": p[1]} for p in op[2]]
        params.append({"_type": "UMLParameter", "_id": gen_id(), "_parent": {"$ref": op_id}, "name": "return", "type": op[1], "direction": "return"})
        op_objs.append({"_type": "UMLOperation", "_id": op_id, "_parent": {"$ref": iid}, "name": op[0], "visibility": "public", "parameters": params})
    
    iface_elem = {
        "_type": "UMLInterface",
        "_id": iid,
        "_parent": {"$ref": model_id},
        "name": iname,
        "attributes": attr_objs,
        "operations": op_objs
    }
    uml_model["ownedElements"].append(iface_elem)

# Classes
class_data = [
    ("CardData", "ScriptableObject", [
        ("CardID", "string"),
        ("CardName", "string"),
        ("CardType", "CardType"),
        ("TargetRange", "TargetRange"),
        ("CardCost", "int"),
        ("Cooldown", "int"),
        ("DamageMultiplier", "float = 1.0f"),
        ("FlatDamage", "int"),
        ("ShieldMultiplier", "float"),
        ("Description", "string"),
        ("ManaCost", "int")
    ], [
        ("ActivateEffect", "void", [("user", "GameObject = null"), ("target", "GameObject = null")])
    ]),
    ("CharacterStats", "ScriptableObject", [
        ("CharacterID", "string"),
        ("CharacterName", "string"),
        ("BaseHP", "int"),
        ("BaseATK", "int"),
        ("BaseDEF", "int"),
        ("APContribution", "int"),
        ("Cards", "List<CardData>")
    ], []),
    ("EnemyStats", "ScriptableObject", [
        ("EnemyID", "string"),
        ("EnemyName", "string"),
        ("IsBoss", "bool"),
        ("BaseHP", "int"),
        ("MaxHealthBars", "int = 1"),
        ("BaseATK", "int"),
        ("BaseDEF", "int"),
        ("IgnoreFrontlineLock", "bool")
    ], []),
    ("BattleUnit", "MonoBehaviour", [
        ("UnitName", "string"),
        ("CharacterData", "CharacterStats"),
        ("EnemyData", "EnemyStats"),
        ("IsPlayerSide", "bool"),
        ("Row", "int"),
        ("Column", "int"),
        ("HasTaunt", "bool"),
        ("IsRooted", "bool"),
        ("IgnoreFrontlineLock", "bool"),
        ("MaxHP", "int"),
        ("currentHP", "int", "private"),
        ("currentShield", "int", "private"),
        ("CurrentATK", "int"),
        ("CurrentDEF", "int"),
        ("APContribution", "int"),
        ("CurrentHP", "int"),
        ("CurrentShield", "int"),
        ("IsDead", "bool")
    ], [
        ("Initialize", "void", [("stats", "CharacterStats"), ("isPlayerSide", "bool = true")]),
        ("Initialize", "void", [("stats", "EnemyStats"), ("isPlayerSide", "bool = false")]),
        ("SetupUnit", "void", [("unitName", "string"), ("hp", "int"), ("atk", "int"), ("def", "int"), ("ap", "int"), ("ignoreFrontline", "bool")]),
        ("TakeDamage", "void", [("amount", "int")]),
        ("AddShield", "void", [("amount", "int")]),
        ("Heal", "void", [("amount", "int")]),
        ("Die", "void", [])
    ])
]

class_refs = {}
for cname, stype, attrs, ops in class_data:
    cid = gen_id()
    class_refs[cname] = cid
    attr_objs = []
    for a in attrs:
        vis = a[2] if len(a) > 2 else "public"
        attr_objs.append({"_type": "UMLAttribute", "_id": gen_id(), "_parent": {"$ref": cid}, "name": a[0], "type": a[1], "visibility": vis})
    
    op_objs = []
    for op in ops:
        op_id = gen_id()
        params = [{"_type": "UMLParameter", "_id": gen_id(), "_parent": {"$ref": op_id}, "name": p[0], "type": p[1]} for p in op[2]]
        params.append({"_type": "UMLParameter", "_id": gen_id(), "_parent": {"$ref": op_id}, "name": "return", "type": op[1], "direction": "return"})
        op_objs.append({"_type": "UMLOperation", "_id": op_id, "_parent": {"$ref": cid}, "name": op[0], "visibility": "public", "parameters": params})
    
    class_elem = {
        "_type": "UMLClass",
        "_id": cid,
        "_parent": {"$ref": model_id},
        "name": cname,
        "stereotype": stype,
        "attributes": attr_objs,
        "operations": op_objs,
        "ownedElements": []
    }
    uml_model["ownedElements"].append(class_elem)

# Add Interface Realization: BattleUnit implements IDamageable
realization_id = gen_id()
# Find BattleUnit elem
for el in uml_model["ownedElements"]:
    if el.get("name") == "BattleUnit":
        el.setdefault("ownedElements", []).append({
            "_type": "UMLInterfaceRealization",
            "_id": realization_id,
            "_parent": {"$ref": el["_id"]},
            "name": "Implements",
            "source": {"$ref": el["_id"]},
            "target": {"$ref": iface_refs["IDamageable"]}
        })
    elif el.get("name") == "CharacterStats":
        # Association: CharacterStats contains CardData
        assoc_id = gen_id()
        el.setdefault("ownedElements", []).append({
            "_type": "UMLAssociation",
            "_id": assoc_id,
            "_parent": {"$ref": el["_id"]},
            "name": "Contains",
            "end1": {
                "_type": "UMLAssociationEnd",
                "_id": gen_id(),
                "_parent": {"$ref": assoc_id},
                "name": "owner",
                "reference": {"$ref": el["_id"]},
                "multiplicity": "1"
            },
            "end2": {
                "_type": "UMLAssociationEnd",
                "_id": gen_id(),
                "_parent": {"$ref": assoc_id},
                "name": "Cards",
                "reference": {"$ref": class_refs["CardData"]},
                "multiplicity": "3",
                "aggregation": "composite"
            }
        })

# ----------------- US-006: State Machine Diagram & Model -----------------
sm_id = gen_id()
region_id = gen_id()
state_diag_id = gen_id()

sm_elem = {
    "_type": "UMLStateMachine",
    "_id": sm_id,
    "_parent": {"$ref": model_id},
    "name": "TurnFlow_StateMachine",
    "regions": [
        {
            "_type": "UMLRegion",
            "_id": region_id,
            "_parent": {"$ref": sm_id},
            "name": "MainTurnRegion",
            "vertices": [],
            "transitions": []
        }
    ],
    "ownedElements": [
        {
            "_type": "UMLStatechartDiagram",
            "_id": state_diag_id,
            "_parent": {"$ref": sm_id},
            "name": "US-006_Turn_Flow_State_Machine",
            "ownedViews": []
        }
    ]
}
uml_model["ownedElements"].append(sm_elem)

reg = sm_elem["regions"][0]

# Vertices
init_id = gen_id()
reg["vertices"].append({
    "_type": "UMLPseudostate",
    "_id": init_id,
    "_parent": {"$ref": region_id},
    "name": "Initial",
    "kind": "initial"
})

states_data = [
    ("BattleInitState", "Khởi tạo lưới 2x3, spawn Tướng & Quái, tính initial AP, TurnCount = 1"),
    ("PlayerTurnState", "Vào lượt người chơi: Hồi phục AP = Sum(Tướng sống), Cooldown -1, Dùng Card / Đổi ô (Swap: tốn 1 Cost), Reset Shield cuối lượt"),
    ("EnemyTurnState", "Vào lượt kẻ địch: Dò tìm mục tiêu (%HP thấp), ra đòn theo Frontline Lock, Reset Shield quái, TurnCount++"),
    ("VictoryState", "Toàn bộ quái vật tử trận - Chiến thắng!"),
    ("DefeatState", "Toàn bộ Tướng phe ta tử trận - Thất bại!")
]

state_map = {"Initial": init_id}
for sname, do_act in states_data:
    sid = gen_id()
    state_map[sname] = sid
    if "State" in sname and "Victory" not in sname and "Defeat" not in sname:
        st_obj = {
            "_type": "UMLState",
            "_id": sid,
            "_parent": {"$ref": region_id},
            "name": sname,
            "doActivity": do_act
        }
    else:
        st_obj = {
            "_type": "UMLFinalState",
            "_id": sid,
            "_parent": {"$ref": region_id},
            "name": sname
        }
    reg["vertices"].append(st_obj)

# Transitions
trans_data = [
    ("Initial", "BattleInitState", "", ""),
    ("BattleInitState", "PlayerTurnState", "Nạp trận đấu hoàn tất", "TurnCount = 1"),
    ("PlayerTurnState", "EnemyTurnState", "Người chơi bấm 'End Turn'", "Reset Player Shield = 0"),
    ("PlayerTurnState", "VictoryState", "Tất cả quái vật tử trận [AllEnemiesDead]", "Kích hoạt Victory Screen"),
    ("EnemyTurnState", "DefeatState", "Tất cả tướng phe ta tử trận [AllPlayerDead]", "Kích hoạt Defeat Screen"),
    ("EnemyTurnState", "PlayerTurnState", "Quái xong đòn đánh [Còn Tướng sống]", "Reset Enemy Shield, TurnCount++")
]

for src, tgt, trigger, effect in trans_data:
    tr_id = gen_id()
    tr_obj = {
        "_type": "UMLTransition",
        "_id": tr_id,
        "_parent": {"$ref": region_id},
        "source": {"$ref": state_map[src]},
        "target": {"$ref": state_map[tgt]},
        "triggers": [{"_type": "UMLEvent", "_id": gen_id(), "_parent": {"$ref": tr_id}, "name": trigger}] if trigger else [],
        "effects": [{"_type": "UMLOpaqueBehavior", "_id": gen_id(), "_parent": {"$ref": tr_id}, "name": effect}] if effect else []
    }
    reg["transitions"].append(tr_obj)

# ----------------- Visual Views for Diagrams -----------------
# Generate Visual Views for Class Diagram
class_positions = {
    "CardData": (50, 60, 260, 280),
    "CharacterStats": (360, 60, 260, 200),
    "EnemyStats": (670, 60, 250, 220),
    "BattleUnit": (360, 320, 340, 360),
    "IDamageable": (760, 360, 230, 180),
    "IStatusEffect": (760, 570, 230, 180),
    "CardType": (50, 380, 200, 140),
    "TargetRange": (50, 550, 220, 160),
    "GridRow": (50, 740, 180, 100)
}

def create_class_view(cname, pos, model_elem):
    cid = gen_id()
    mid = model_elem["_id"]
    left, top, w, h = pos
    
    # Subviews
    name_comp_id = gen_id()
    name_label_id = gen_id()
    subviews = [
        {
            "_type": "UMLNameCompartmentView",
            "_id": name_comp_id,
            "_parent": {"$ref": cid},
            "model": {"$ref": mid},
            "subViews": [
                {
                    "_type": "LabelView",
                    "_id": name_label_id,
                    "_parent": {"$ref": name_comp_id},
                    "font": "Arial;13;1",
                    "left": left,
                    "top": top + 5,
                    "width": w,
                    "height": 20,
                    "text": cname,
                    "horizontalAlignment": 2
                }
            ],
            "left": left,
            "top": top,
            "width": w,
            "height": 30
        }
    ]
    
    # Attributes compartment
    attr_comp_id = gen_id()
    attr_subviews = []
    if "attributes" in model_elem:
        curr_y = top + 35
        for attr in model_elem["attributes"]:
            avid = gen_id()
            attr_subviews.append({
                "_type": "UMLAttributeView",
                "_id": avid,
                "_parent": {"$ref": attr_comp_id},
                "model": {"$ref": attr["_id"]},
                "font": "Arial;11;0",
                "left": left + 5,
                "top": curr_y,
                "width": w - 10,
                "height": 14,
                "text": f"{attr.get('visibility','+')} {attr['name']}: {attr['type']}"
            })
            curr_y += 15
    elif "literals" in model_elem:
        curr_y = top + 35
        for lit in model_elem["literals"]:
            lvid = gen_id()
            attr_subviews.append({
                "_type": "UMLEnumerationLiteralView",
                "_id": lvid,
                "_parent": {"$ref": attr_comp_id},
                "model": {"$ref": lit["_id"]},
                "font": "Arial;11;0",
                "left": left + 5,
                "top": curr_y,
                "width": w - 10,
                "height": 14,
                "text": lit['name']
            })
            curr_y += 15
            
    subviews.append({
        "_type": "UMLAttributeCompartmentView",
        "_id": attr_comp_id,
        "_parent": {"$ref": cid},
        "model": {"$ref": mid},
        "subViews": attr_subviews,
        "left": left,
        "top": top + 30,
        "width": w,
        "height": max(20, len(attr_subviews) * 15 + 10)
    })
    
    # Operations compartment
    if "operations" in model_elem and model_elem["operations"]:
        op_comp_id = gen_id()
        op_subviews = []
        curr_y = top + 30 + max(20, len(attr_subviews) * 15 + 10) + 5
        for op in model_elem["operations"]:
            ovid = gen_id()
            op_subviews.append({
                "_type": "UMLOperationView",
                "_id": ovid,
                "_parent": {"$ref": op_comp_id},
                "model": {"$ref": op["_id"]},
                "font": "Arial;11;0",
                "left": left + 5,
                "top": curr_y,
                "width": w - 10,
                "height": 14,
                "text": f"+ {op['name']}()"
            })
            curr_y += 15
        subviews.append({
            "_type": "UMLOperationCompartmentView",
            "_id": op_comp_id,
            "_parent": {"$ref": cid},
            "model": {"$ref": mid},
            "subViews": op_subviews,
            "left": left,
            "top": top + 30 + max(20, len(attr_subviews) * 15 + 10),
            "width": w,
            "height": max(20, len(op_subviews) * 15 + 10)
        })

    view_type = "UMLClassView"
    if model_elem["_type"] == "UMLInterface":
        view_type = "UMLInterfaceView"
    elif model_elem["_type"] == "UMLEnumeration":
        view_type = "UMLEnumerationView"

    return {
        "_type": view_type,
        "_id": cid,
        "_parent": {"$ref": class_diag_id},
        "model": {"$ref": mid},
        "subViews": subviews,
        "font": "Arial;12;0",
        "left": left,
        "top": top,
        "width": w,
        "height": h
    }

for el in uml_model["ownedElements"]:
    name = el.get("name")
    if name in class_positions:
        class_diagram["ownedViews"].append(create_class_view(name, class_positions[name], el))

# Generate Visual Views for Statechart Diagram
state_positions = {
    "Initial": (80, 200, 25, 25),
    "BattleInitState": (160, 175, 230, 80),
    "PlayerTurnState": (460, 120, 320, 190),
    "EnemyTurnState": (460, 370, 320, 160),
    "VictoryState": (880, 160, 180, 70),
    "DefeatState": (880, 410, 180, 70)
}

for vert in reg["vertices"]:
    vname = vert.get("name")
    if vname in state_positions:
        left, top, w, h = state_positions[vname]
        vid = gen_id()
        if vert["_type"] == "UMLPseudostate":
            v_type = "UMLPseudostateView"
            view_obj = {
                "_type": v_type,
                "_id": vid,
                "_parent": {"$ref": state_diag_id},
                "model": {"$ref": vert["_id"]},
                "left": left, "top": top, "width": w, "height": h
            }
        elif vert["_type"] == "UMLFinalState":
            v_type = "UMLFinalStateView"
            view_obj = {
                "_type": v_type,
                "_id": vid,
                "_parent": {"$ref": state_diag_id},
                "model": {"$ref": vert["_id"]},
                "left": left, "top": top, "width": w, "height": h
            }
        else:
            v_type = "UMLStateView"
            view_obj = {
                "_type": v_type,
                "_id": vid,
                "_parent": {"$ref": state_diag_id},
                "model": {"$ref": vert["_id"]},
                "subViews": [
                    {
                        "_type": "UMLNameCompartmentView",
                        "_id": gen_id(),
                        "_parent": {"$ref": vid},
                        "model": {"$ref": vert["_id"]},
                        "subViews": [
                            {
                                "_type": "LabelView",
                                "_id": gen_id(),
                                "font": "Arial;13;1",
                                "left": left, "top": top + 5, "width": w, "height": 18,
                                "text": vname,
                                "horizontalAlignment": 2
                            }
                        ],
                        "left": left, "top": top, "width": w, "height": 25
                    }
                ],
                "left": left, "top": top, "width": w, "height": h
            }
        state_diag = sm_elem["ownedElements"][0]
        state_diag["ownedViews"].append(view_obj)

# Save to file
out_path = r'Docs\UML\Antigravity_StarUML_Project.mdj'
with open(out_path, 'w', encoding='utf-8') as f:
    json.dump(project, f, indent=2, ensure_ascii=False)

print(f"Successfully generated StarUML Project: {out_path}")
