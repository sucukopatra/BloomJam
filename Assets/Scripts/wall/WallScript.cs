using System;
using System.Collections;
using BloomJam.Enemies;
using UnityEngine;
using YigitcanCaliskan.EventBus;
using Random = System.Random;

[RequireComponent(typeof(Renderer))]
public class WallScript : MonoBehaviour
{
    [Serializable]
    public struct WallFrame
    {
        public Texture base_map;
        public Texture normal_map;
        public Texture metallic_map;
    }

    [SerializeField] private texture_memory texture_memory;
   
    [SerializeField] private WallFrame red1;
    [SerializeField] private WallFrame gren1;
    [SerializeField] private WallFrame blue1;
    [SerializeField] private WallFrame red2;
    [SerializeField] private WallFrame gren2;
    [SerializeField] private WallFrame blue2; 
    [SerializeField] private WallFrame red3;
    [SerializeField] private WallFrame gren3;
    [SerializeField] private WallFrame blue3;
    [SerializeField] private EnemyType triggerType;
    [SerializeField] private int pairingId;
    [SerializeField] private WallFrame[] frames;
    [SerializeField] private WallFrame finish_frame;
    [SerializeField] private float frames_speed;

    private Renderer _renderer;
    private MaterialPropertyBlock _block;
    private bool _alreadyActivated;
    private int finis_color_id;

    private void Awake()
    {
       // finis_color_id= Random.Range(0, 4);
        texture_memory=transform.parent.transform.parent.GetComponent<texture_memory>();

        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
        red1 = texture_memory.red1;
        gren1 = texture_memory.gren1;
        blue1 = texture_memory.blue1;
        
        if (triggerType==EnemyType.Red)
        {
            finish_frame = red1;

        }
        else
        {
            if (triggerType==EnemyType.Blue)
            {
                finish_frame = blue1;

            }
            else
            {
                if (triggerType==EnemyType.Green)
                {
                    finish_frame = gren1;

                }
                else
                {
            
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
          //  ApplyFrame(finish_frame);

        }
    }

    private void OnEnable()  => EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    private void OnDisable() => EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (_alreadyActivated) return;
        if (evt.Type != triggerType) return;
        if (evt.PairingId != pairingId) return;

        _alreadyActivated = true;
        StartCoroutine(PlayFrames());
    }

    private IEnumerator PlayFrames()
    {
        foreach (var frame in frames)
        {
            ApplyFrame(frame);
            yield return new WaitForSeconds(frames_speed);
        }
        ApplyFrame(finish_frame);
    }

    private void ApplyFrame(WallFrame frame)
    {
      //  Debug.Log($"base:{frame.base_map}  normal:{frame.normal_map}  metallic:{frame.metallic_map}");
        _renderer.GetPropertyBlock(_block);
        if (frame.base_map     != null) _block.SetTexture("_BaseMap",          frame.base_map);
        if (frame.normal_map   != null) _block.SetTexture("_BumpMap",          frame.normal_map);
        if (frame.metallic_map != null) _block.SetTexture("_MetallicGlossMap", frame.metallic_map);
        _renderer.SetPropertyBlock(_block);
    }
}