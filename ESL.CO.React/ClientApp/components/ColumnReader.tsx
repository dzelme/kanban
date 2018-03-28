import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { Value, Board, Credentials } from './Interfaces';
import { ApiClient } from './ApiClient';

interface ColumnReaderState {
    boardlist: Value[];
    currentIndex: number;
    boardId: number;
    board: Board;
    boardChanged: boolean;
    loading: boolean;
    titleList: string[];
    credentials: Credentials;
}

// test when no appSettings.json - currently creates error @boardId: this.props.boardlist[0].id
// error because generated file hass all boards with visibility false
export default class ColumnReader extends React.Component<{ boardlist: Value[], presID: string, titleList: string[] }, ColumnReaderState> {
    refreshTimer: number;
    
    constructor(props) {
        super(props);
        this.state = {
            boardlist: this.props.boardlist,
            currentIndex: 0,
            boardId: this.props.boardlist[0].id,
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            loading: true,
            titleList: this.props.titleList,
            credentials: { username: "service.kosmoss.tv", password: "ZycsakMylp8od6" }
        };

        this.nextSlide = this.nextSlide.bind(this);

        if (this.props.presID != '0') {
            ApiClient.getAPresentation(this.props.presID)
                .then(dataPres => {

                    ApiClient.boardData(this.state.boardId, dataPres.credentials)
                        .then(dataBoard => {
                              this.setState({ board: dataBoard, loading: false, boardChanged: true }, this.RefreshRate);
                        });
                });
        }
        else {
            ApiClient.boardData(this.state.boardId, this.state.credentials)
                .then(dataBoard => {
                      this.setState({ board: dataBoard, boardChanged: true }, this.makeTitleList);
                });
        }
    }

    makeTitleList() {
        let list = [];

        list.push(this.state.board.name);
        
        this.setState({ titleList: list, loading: false }, this.RefreshRate);
    }

    nextSlide() {

        var index;

        if (this.state.currentIndex == (this.state.boardlist.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index,
            boardId: this.state.boardlist[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }


    slideShow() {
        this.increment();  //AD: increments timesShown board statistic
        setTimeout(this.nextSlide, this.state.boardlist[this.state.currentIndex].timeShown);
    }

    boardLoad() {
        clearInterval(this.refreshTimer);

        if (this.props.presID != '0') {
            ApiClient.getAPresentation(this.props.presID)
                .then(dataPres => {

                    ApiClient.boardData(this.state.boardId, dataPres.credentials)
                        .then(dataBoard => {
                            if (dataBoard.id == this.state.boardId) {
                                if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false) {
                                    this.setState({ board: dataBoard, boardChanged: false }, this.RefreshRate);
                                }
                                else {
                                    this.setState({ board: dataBoard, boardChanged: true }, this.RefreshRate);
                                }
                            }
                        });
                });
        }
        else {
            ApiClient.boardData(this.state.boardId, this.state.credentials)
                .then(data => {

                      if (this.state.board.id == data.id && data.hasChanged == false) {
                          this.setState({ board: data, boardChanged: false }, this.RefreshRate);
                      }
                      else {
                          this.setState({ board: data, boardChanged: true }, this.RefreshRate);
                      }
                
                });
        }


        
    }

    RefreshRate() {
        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardlist[this.state.currentIndex].refreshRate
        );
    }

    //AD: increments timesShown board statistic
    increment() {
        ApiClient.incrementTimesShown(this.state.boardId);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!</h1>
            }
            else {
                return <div>

                    <div>  <BoardName name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} allNames={this.state.titleList} /></div>
                    <div id='board'><BoardTable board={this.state.board} /></div>

                    { (this.state.boardlist.length <= 1) ? this.increment() : this.slideShow() }

                </div>;
            }
        }
    }
}