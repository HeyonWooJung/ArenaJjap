using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBase
{
    protected PlayerController controller;
    protected Character character;
    public virtual void Execute()
    {

    }

    public CommandBase(PlayerController controller)
    {
        this.controller = controller;
        character = controller.character;
    }
}

public class MoveCommand : CommandBase //움직임
{
    Vector3 movePos;
    public MoveCommand(PlayerController controller, Vector3 movePos) : base(controller)
    {
        this.movePos = movePos;
    }

    public override void Execute()
    {
        controller.Move(movePos);
    }

}

public class AutoAttackCommand : CommandBase //평타
{
    PlayerController target;
    public AutoAttackCommand(PlayerController controller, PlayerController target) : base(controller)
    {
        this.target = target;
    }

    public override void Execute()
    {
        controller.AutoAttack(target);
    }
}

public class RushCommand : CommandBase //도주(유체화)
{
    public RushCommand(PlayerController controller) : base(controller)
    {

    }

    public override void Execute()
    {
        controller.Rush();
    }
}


public class FlashCommmand : CommandBase //점멸
{
    Vector3 pos;

    public FlashCommmand(PlayerController controller, Vector3 pos) : base(controller)
    {
        this.pos = pos;
    }

    public override void Execute()
    {
        controller.Flash(pos);
    }
}

public class SKillCommands : CommandBase //스킬들 베이스
{
    protected bool isTargeting;
    protected bool isChanneling;
    protected PlayerController target;
    protected Vector3 location;

    public SKillCommands(PlayerController controller, bool isTargeting, bool isChanneling, PlayerController target, Vector3 location) : base(controller)
    {
        this.isTargeting = isTargeting;
        this.isChanneling = isChanneling;
        this.target = target;
        this.location = location;
    }
}

public class SkillQCommand : SKillCommands //Q스킬
{
    public SkillQCommand(PlayerController controller, bool isTargeting, bool isChanneling, PlayerController target, Vector3 location) : base(controller, isTargeting, isChanneling, target, location)
    {

    }

    public override void Execute()
    {
        controller.SkillQ(isTargeting, isChanneling, target, location);
    }
}

public class SkillWCommand : SKillCommands //W스킬
{
    public SkillWCommand(PlayerController controller, bool isTargeting, bool isChanneling, PlayerController target, Vector3 location) : base(controller, isTargeting, isChanneling, target, location)
    {

    }

    public override void Execute()
    {
        controller.SkillW(isTargeting, isChanneling, target, location);
    }
}

public class SkillECommand : SKillCommands //E스킬
{
    public SkillECommand(PlayerController controller, bool isTargeting, bool isChanneling, PlayerController target, Vector3 location) : base(controller, isTargeting, isChanneling, target, location)
    {

    }

    public override void Execute()
    {
        controller.SkillE(isTargeting, isChanneling, target, location);
    }
}

public class SkillRCommand : SKillCommands //R스킬
{
    public SkillRCommand(PlayerController controller, bool isTargeting, bool isChanneling, PlayerController target, Vector3 location) : base(controller, isTargeting, isChanneling, target, location)
    {

    }

    public override void Execute()
    {
        controller.SkillR(isTargeting, isChanneling, target, location);
    }
}