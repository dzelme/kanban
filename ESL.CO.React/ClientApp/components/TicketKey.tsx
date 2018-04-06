import * as React from 'react';

export default class TicketKey extends React.Component<{ keyName: string }> {
    public render() {

        return <div className="TicketText">
            <h5>{this.props.keyName}</h5>
        </div>
    }
}