using IEMod.Mods.Options;
using Patchwork.Attributes;
using UnityEngine;

namespace IEMod.Mods.HideCape
{
    [ModifiesType]
    public class mod_NPCAppearance : NPCAppearance
    {
        [ModifiesMember("AttachCape")]
        public void AttachCapeNew(Transform skeleton)
        {
            if (!IEModOptions.CapesHidden) {// Only inserted line really...
                Equipment component = base.GetComponent<Equipment>();
                if (component == null)
                {
                    return;
                }
                Equippable neck = null;
                if (component.CurrentItems != null)
                {
                    if (component.CurrentItems.Neck != null && component.CurrentItems.Neck.Appearance.bodyPiece == AppearancePiece.BodyPiece.Cape)
                    {
                        neck = component.CurrentItems.Neck;
                    }
                }
                else if (component.DefaultEquippedItems != null && component.DefaultEquippedItems.Neck != null && component.DefaultEquippedItems.Neck.Appearance.bodyPiece == AppearancePiece.BodyPiece.Cape)
                {
                    neck = component.DefaultEquippedItems.Neck;
                }
                if (neck == null)
                {
                    if (this.m_capeMeshObject != null)
                    {
                        GameUtilities.Destroy(this.m_capeMeshObject);
                        this.m_capeMeshObject = null;
                    }
                    this.m_capeCachedEquippable = null;
                    return;
                }
                if (this.m_capeCachedEquippable == neck)
                {
                    return;
                }
                if (this.m_capeMeshObject != null)
                {
                    GameUtilities.Destroy(this.m_capeMeshObject);
                    this.m_capeMeshObject = null;
                }
                this.m_capeCachedEquippable = neck;
                if (this.FindBone(skeleton, "bn_cloth_01") == null)
                {
                    this.AddCapeBone(this.FindBone(skeleton, "Neck"));
                }
                string capePrefabPath = this.GetCapePrefabPath();
                NPCAppearance.s_loader.LoadBundle<GameObject>(capePrefabPath, false);
                if (NPCAppearance.s_loader.obj)
                {
                    this.m_capeMeshObject = Object.Instantiate(NPCAppearance.s_loader.obj) as GameObject;
                    this.m_capeMeshObject.name = "Cape Mesh";
                    this.m_capeMeshObject.transform.parent = base.transform;
                    this.m_capeMeshObject.transform.localPosition = Vector3.zero;
                    this.m_capeMeshObject.transform.localRotation = Quaternion.identity;
                    if (base.gameObject.layer != PE_Paperdoll.PaperdollLayer)
                    {
                        this.m_capeMeshObject.layer = LayerUtility.FindLayerValue("Dynamics");
                    }
                    else
                    {
                        this.m_capeMeshObject.layer = PE_Paperdoll.PaperdollLayer;
                    }
                    ClothMesh clothMesh = this.m_capeMeshObject.GetComponent<ClothMesh>();
                    if (clothMesh != null)
                    {
                        clothMesh.SkeletonObject = skeleton.gameObject;
                        clothMesh.SkinnedMeshes = new SkinnedMeshRenderer[1];
                        SkinnedMeshRenderer sLoader = this.m_capeMeshObject.GetComponent<SkinnedMeshRenderer>();
                        string str = "Art/Character/Textures/Cape/m_Cape01_V";
                        str = (neck.Appearance.materialVariation >= 10 ? string.Concat(str, neck.Appearance.materialVariation, string.Empty) : string.Concat(str, "0", neck.Appearance.materialVariation));
                        NPCAppearance.s_loader.LoadBundle<GameObject>(str, false);
                        if (!NPCAppearance.s_loader.obj)
                        {
                            Debug.LogError(string.Concat("Cape Material Asset could not be found! Searched for at: '", str, "'"));
                        }
                        else
                        {
                            sLoader.material = NPCAppearance.s_loader.obj as Material;
                        }
                        clothMesh.SkinnedMeshes[0] = sLoader;
                    }
                }
            }
        }
    }
}
