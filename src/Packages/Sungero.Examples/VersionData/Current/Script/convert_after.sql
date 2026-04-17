update
    sungero_core_recipient
set
    notifichannel_example_sungero = 'DoNotNotify'
where
    notifichannel_example_sungero is null
    and discriminator = 'b7905516-2be5-4931-961c-cb38d5677565'
