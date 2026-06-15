namespace titfortat.MessagesScheduler
open Sharpino.Core
open Sharpino
open System.Text.Json
open FsToolkit.ErrorHandling
open titfortat.Shared
open TitForTat.Shared.Commons
open System

type MailQueueCommand =
    | AddMailQueueItem of MailQueueItem
    | RemoveMailQueueItem of MailQueueItemId
    | IncrementRetryCount of MailQueueItemId
    interface AggregateCommand<MailQueue, MailQueueEvent> with
        member this.Execute (state: MailQueue) =
            match this with
            | AddMailQueueItem item -> 
                state.AddMailQueueItem(item) 
                |> Result.map (fun s -> (s, [MailQueueEvent.MailQueueItemAdded item]))
            | RemoveMailQueueItem mailQueueItemId -> 
                state.RemoveMailQueueItem (mailQueueItemId) 
                |> Result.map (fun x -> (x, [MailQueueEvent.MailQueueItemRemoved mailQueueItemId]))
            | IncrementRetryCount mailQueueItemId -> 
                state.IncrementRetryCount(mailQueueItemId) 
                |> Result.map (fun x -> (x, [MailQueueEvent.MailQueueItemRetryCountIncremented mailQueueItemId]))
        member this.Undoer =
            None