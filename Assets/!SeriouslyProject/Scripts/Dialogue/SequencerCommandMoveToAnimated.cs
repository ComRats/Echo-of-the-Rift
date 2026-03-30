using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    [AddComponentMenu("")]
    public class SequencerCommandMoveToAnimated : SequencerCommand
    {
        private const float SmoothMoveCutoff = 0.05f;

        private Transform target;
        private Transform subject;
        private Rigidbody subjectRigidbody;
        private Animator animator;
        private float duration;
        private float startTime;
        private float endTime;
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        public void Start()
        {
            target = GetSubject(0);
            subject = GetSubject(1);
            duration = GetParameterAsFloat(2, 0);

            animator = subject != null ? subject.GetComponent<Animator>() : null;
            subjectRigidbody = subject != null ? subject.GetComponent<Rigidbody>() : null;

            if ((subject != null) && (target != null) && (subject != target))
            {
                if (duration > SmoothMoveCutoff)
                {
                    startTime = DialogueTime.time;
                    endTime = startTime + duration;
                    originalPosition = subject.position;
                    originalRotation = subject.rotation;
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }

        private void SetPosition(Vector3 newPosition, Quaternion newRotation)
        {
            if (subjectRigidbody != null && !subjectRigidbody.isKinematic)
            {
                subjectRigidbody.MoveRotation(newRotation);
                subjectRigidbody.MovePosition(newPosition);
            }
            else
            {
                subject.rotation = newRotation;
                subject.position = newPosition;
            }
        }

        public void Update()
        {
            if (DialogueTime.time < endTime)
            {
                float elapsed = (DialogueTime.time - startTime) / duration;
                Vector3 newPos = Vector3.Lerp(originalPosition, target.position, elapsed);
                Quaternion newRot = Quaternion.Lerp(originalRotation, target.rotation, elapsed);
                SetPosition(newPos, newRot);

                // --- ANIMATION LOGIC ---
                if (animator != null)
                {
                    Vector3 dir = (target.position - subject.position).normalized;
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                    {
                        if (dir.x > 0.1f) animator.Play("WalkRight");
                        else if (dir.x < -0.1f) animator.Play("WalkLeft");
                    }
                    else
                    {
                        if (dir.z > 0.1f) animator.Play("WalkForward");
                        else if (dir.z < -0.1f) animator.Play("WalkBack");
                    }
                }
            }
            else
            {
                // Финальная позиция и Idle
                SetPosition(target.position, target.rotation);
                if (animator != null) animator.Play("Idle");
                Stop();
            }
        }

        public void OnDestroy()
        {
            if ((subject != null) && (target != null) && (subject != target))
            {
                SetPosition(target.position, target.rotation);
                if (animator != null) animator.Play("Idle");
            }
        }
    }
}