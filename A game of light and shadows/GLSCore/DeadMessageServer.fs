module GLSPersistance.DeadMessageServer

// Akka doesn’t guarantee the delivery of a message
// The dead letter channel is an event stream which receives all messages that couldn't be processed
// Unless specified differently, dead letters are logged in the INFO level
