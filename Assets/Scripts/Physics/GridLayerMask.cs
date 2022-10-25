using System;

namespace BlueNoah.Common
{

    public class GridLayerMask 
    {

        int _LayerMask = 0;

        public int LayerMask
        {
            get
            {
                return _LayerMask;
            }
        }
        public GridLayerMask(int layerMask)
        {
            _LayerMask = layerMask;
        }

        public GridLayerMask() { }
        //レーヤーを追加(layer:0-31)
        public void AddLayer(uint layer)
        {
            if (layer < 0 || layer > 31)
            {
                return;
            }
            AddLayers(1 << (int)layer);
        }
        //例：AddLayers( ~(1 << 2 | 1 << 5)) 
        public void AddLayers(int layerMask)
        {
            _LayerMask = _LayerMask | layerMask;
        }

        public void RemoveLayer(uint layer)
        {
            if (layer < 1 || layer > 31)
            {
                return;
            }
            RemoveLayers(1 << (int)layer);
        }
        //例：RemoveLayers( ~(1 << 2 | 1 << 5)) 
        public void RemoveLayers(int layerMask)
        {
            _LayerMask = (_LayerMask & (_LayerMask ^ layerMask));
        }
        //素早くに判断
        [Obsolete("Use 'ContainLayer' insteat", true)]
        public bool ContainLayerOld(int layer)
        {
            return layer == 0 || (_LayerMask >> layer) % 2 == 1;
        }
        //Inspect and verify the mask whether valid in target mask list
        [Obsolete("Use 'ValidateLyaerMask' insteat",true)]
        public static bool ValidateLayerMaskOld(int layerMask,int layer)
        {
            if (layer < 0 || layer > 31)
            {
                return false;
            }
            //「>>」は「%」より、優先度が低い
            return layerMask == 0 || (layerMask >> layer) % 2 == 1;
        }

        public static bool ValidateLayerMask(int sourceLayerMask, int layerMask)
        {
            if (sourceLayerMask > 0)
            {
                return (sourceLayerMask & layerMask) > 0;
            }
            return true;
        }
        //例：ContainLayer( ~(1 << 2 | 1 << 5)) 
        public bool ContainLayer(int layerMask)
        {
            return (_LayerMask & layerMask) > 0;
        }

        public static implicit operator GridLayerMask(int value)
        {
            return new GridLayerMask(value);
        }

        public static implicit operator int(GridLayerMask gridLayerMask)
        {
            return gridLayerMask._LayerMask;
        }
    }
}
