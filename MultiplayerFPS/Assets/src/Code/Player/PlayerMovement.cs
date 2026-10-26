using UnityEngine;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    private float verticalVelocity;

    public struct MoveData : IReplicateData
    {
        public Vector2 Move;
        public bool Jump;

        private uint tick;

        public MoveData(Vector2 move, bool jump)
        {
            Move = move;
            Jump = jump;
            tick = 0;
        }

        public void Dispose() { }
        public uint GetTick() => tick;
        public void SetTick(uint value) => tick = value;
    }

    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        public float VerticalVelocity;

        private uint tick;

        public ReconcileData(Vector3 position, float velocity)
        {
            Position = position;
            VerticalVelocity = velocity;
            tick = 0;
        }

        public void Dispose() { }
        public uint GetTick() => tick;
        public void SetTick(uint value) => tick = value;
    }

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();
    }

    public override void OnStartNetwork()
    {
        base.TimeManager.OnTick += OnTick;
        base.TimeManager.OnPostTick += OnPostTick;
    }

    public override void OnStopNetwork()
    {
        base.TimeManager.OnTick -= OnTick;
        base.TimeManager.OnPostTick -= OnPostTick;
    }

    private void OnTick()
    {
        MoveData data = default;

        if (IsOwner && inputReader != null)
        {
            data = new MoveData(
                inputReader.Move,
                inputReader.JumpPressed
            );

            inputReader.ConsumeJump();
        }

        Move(data);
    }

    [Replicate]
    private void Move(
        MoveData data,
        ReplicateState state = ReplicateState.Invalid,
        Channel channel = Channel.Unreliable)
    {
        float dt = (float)base.TimeManager.TickDelta;

        Vector3 move = transform.right * data.Move.x
                     + transform.forward * data.Move.y;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (data.Jump && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * dt;

        move.y = verticalVelocity;
        controller.Move(move * moveSpeed * dt);
    }

    private void OnPostTick()
    {
        CreateReconcile();
    }

    public override void CreateReconcile()
    {
        ReconcileState(new ReconcileData(
            transform.position,
            verticalVelocity
        ));
    }

    [Reconcile]
    private void ReconcileState(
        ReconcileData data,
        Channel channel = Channel.Unreliable)
    {
        // CharacterController must be disabled while restoring position.
        controller.enabled = false;
        transform.position = data.Position;
        controller.enabled = true;

        verticalVelocity = data.VerticalVelocity;
    }
}
